using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EVRC.Core;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ControlBindingsManagerTests
{
    private GameObject parentGameObject;
    private ControlBindingsManager controlBindingsManager;
    private ControlBindingsState controlBindingsState;
    private GameEvent bindingsChangedEvent;
    private List<string> tempFiles;
    private string tempBindingsPath;

    [SetUp]
    public void Setup()
    {
        // Create a new empty scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        // Set the newly created scene as the active scene
        SceneManager.SetActiveScene(newScene);

        parentGameObject = new GameObject("Test Control Binding Manager");
        controlBindingsManager = parentGameObject.AddComponent<ControlBindingsManager>();
        controlBindingsState = ScriptableObject.CreateInstance<ControlBindingsState>();
        bindingsChangedEvent = ScriptableObject.CreateInstance<GameEvent>();

        controlBindingsManager.controlBindingsState = controlBindingsState;
        controlBindingsManager.controlBindingsState.gameEvent = bindingsChangedEvent;
        controlBindingsManager.eliteBindingsLoadedEvent = ScriptableObject.CreateInstance<GameEvent>();

        tempFiles = new List<string>();

        // Make a temp copy of the template bindings file
        tempBindingsPath = Path.Combine(Application.temporaryCachePath, "TempBindings.binds");
        File.Copy(Paths.BindingsTemplatePath, tempBindingsPath);
        tempFiles.Add(tempBindingsPath);

        controlBindingsManager.SetBindingsFile(tempBindingsPath);
    }

    [Test]
    public void LoadBindings_Updates_ScriptableObject()
    {
        // Arrange
        var startCount = controlBindingsState.buttonBindings.Count;

        // Act
        controlBindingsManager.LoadControlBindings();

        // Assert
        Assert.AreNotEqual(startCount, controlBindingsState.buttonBindings.Count);
    }

    [Test]
    public void BindingsWatcher_InvokesEvent_AfterFileChange()
    {
        // Arrange
        // Start watching (through reload method, which calls the private Watch method)
        controlBindingsManager.Reload();

        XDocument doc = XDocument.Load(tempBindingsPath);

        // Find the YawLeftButton element
        XElement yawLeftButton = doc.Descendants("YawLeftButton").FirstOrDefault();

        // If the YawLeftButton element exists, update the Primary element
        if (yawLeftButton != null)
        {
            XElement primary = yawLeftButton.Descendants("Primary").FirstOrDefault();
            if (primary != null)
            {
                // Update the Key attribute
                primary.Attribute("Key")!.Value = "Key_W";
            }
        }

        // Act
        doc.Save(tempBindingsPath); // should kick off a re-read of the bindings
        // yield return null;

        bool eventInvoked = false;
        bindingsChangedEvent.Event += () => eventInvoked = true;

        Assert.That(
            () => eventInvoked,
            Is.True.After(5000),
            "BindingsChanged event was not invoked within 5 seconds after file change.");
    }

    [TearDown]
    public void TearDown()
    {
        foreach (string file in tempFiles)
        {
            File.Delete(file);
        }
    }
}
