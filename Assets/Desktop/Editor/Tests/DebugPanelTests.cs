using EVRC.Core;
using EVRC.Desktop;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugPanelTests : MonoBehaviour
{
    public GameObject parentObject;
    public EliteStatusViewController statusViewController;
    public EliteDangerousState eliteDangerousState;
    private VisualElement visualElement;
    private Label timestamp;
    private Label guiFocus;
    private Label statusFlags;
    private Label processActive;
    private Label processName;
    private Label processId;
    private Label processDir;

    [SetUp]
    public void Setup()
    {
        // Create a new View and UI Document
        parentObject = new GameObject("Parent");
        UIDocument uIDocument = parentObject.AddComponent<UIDocument>();
        

        // Instantiate VisualElement and add the expected Label objects
        visualElement = new VisualElement();
        var timestampObject = new Label("Timestamp") { name = "timestamp-value" };
        var guiFocusObject = new Label("GuiFocus") { name = "guifocus-value" };
        var statusFlagsObject = new Label("StatusFlags") { name = "statusflags-value" };
        var processactiveObject = new Label("ProcessActive") { name = "process-active-value" };
        var processnameObject = new Label("ProcessName") { name = "process-name-value" };
        var processidObject = new Label("ProcessId") { name = "process-id-value" };
        var processdirectoryObject = new Label("ProcessDirectory") { name = "process-directory-value" };
        visualElement.Add(timestampObject);
        visualElement.Add(guiFocusObject);
        visualElement.Add(statusFlagsObject);
        visualElement.Add(processactiveObject);
        visualElement.Add(processnameObject);
        visualElement.Add(processidObject);
        visualElement.Add(processdirectoryObject);
        // Add the visualElement as the root object
        uIDocument.rootVisualElement.Add(visualElement);

        // Query for the Label object with the name "test-label"
        timestamp = visualElement.Q<Label>("timestamp-value");
        guiFocus = visualElement.Q<Label>("guifocus-value");
        statusFlags = visualElement.Q<Label>("statusflags-value");
        processActive = visualElement.Q<Label>("process-active-value");
        processName = visualElement.Q<Label>("process-name-value");
        processId = visualElement.Q<Label>("process-id-value");
        processDir = visualElement.Q<Label>("process-directory-value");

        statusViewController = parentObject.AddComponent<EliteStatusViewController>();
        eliteDangerousState = ScriptableObject.CreateInstance<EliteDangerousState>();
        statusViewController.eliteDangerousState = eliteDangerousState;
        statusViewController.parentUIDocument = uIDocument;
        
        // Run OnEnable to make sure references are valid
        statusViewController.OnEnable();
    }

    [Test]
    public void StatusChange_Updates_Timestamp()
    {
        // -------------- ARRANGE -----------------------
        // Make sure we're starting with a clean slate for the eliteDangerousState
        Assert.AreEqual(eliteDangerousState.lastStatusFromFile.timestamp, null);
        // Save the starting value from the Desktop UI element
        string startText = timestamp.text;

        #region -------------- ACT -----------------------
        string testDateString = DateTime.Now.ToString();
        // Change the value of the eliteDangerousState
        eliteDangerousState.lastStatusFromFile.timestamp = testDateString;
        // Initiate a refresh, which would normally be handled by a Listener component
        statusViewController.Refresh();

        #endregion

        // --------------- ASSERT --------------------
        string endText = timestamp.text;
        Assert.AreEqual(testDateString, endText);
    }

    [Test]
    public void StatusChange_Updates_GuiFocus()
    {
        // -------------- ARRANGE -----------------------
        // Make sure we're starting with a clean slate for the eliteDangerousState
        Assert.AreEqual(eliteDangerousState.guiFocus, EDGuiFocus.NoFocus);
        // Save the starting value from the Desktop UI element
        string startText = guiFocus.text;

        #region -------------- ACT -----------------------
        string testGuiFocusString = EDGuiFocus.FSSMode.ToString();
        // Change the value of the eliteDangerousState
        eliteDangerousState.guiFocus = EDGuiFocus.FSSMode;
        // Initiate a refresh, which would normally be handled by a Listener component
        statusViewController.Refresh();

        #endregion

        // --------------- ASSERT --------------------
        string endText = guiFocus.text;
        Assert.AreEqual(testGuiFocusString, endText);
        Assert.AreEqual("FSSMode", endText); // should be a readable string, not an integer
    }

    [Test]
    public void StatusChange_Updates_StatusFlags()
    {
        // -------------- ARRANGE -----------------------
        // Make sure we're starting with a clean slate for the eliteDangerousState
        Assert.AreEqual(eliteDangerousState.statusFlags, default(EDStatusFlags));
        // Save the starting value from the Desktop UI element
        string startText = statusFlags.text;

        #region -------------- ACT -----------------------
        string testStatusFlagsString = EDStatusFlags.InMainShip.ToString();
        // Change the value of the eliteDangerousState
        eliteDangerousState.statusFlags = EDStatusFlags.InMainShip;
        // Initiate a refresh, which would normally be handled by a Listener component
        statusViewController.Refresh();

        #endregion

        // --------------- ASSERT --------------------
        string endText = statusFlags.text;
        Assert.AreEqual(testStatusFlagsString, endText);
    }

    [Test]
    public void StatusFlags_Format_ForHumans()
    {
        // -------------- ARRANGE -----------------------
        EDStatusFlags statusFlags = EDStatusFlags.InMainShip | EDStatusFlags.Supercruise | EDStatusFlags.BeingInterdicted;
        string expectedText = "Supercruise | BeingInterdicted | InMainShip";

        // -------------- ACT -----------------------
        string resultText = statusViewController.FormatStatusFlagsForHumans<EDStatusFlags>(statusFlags);

        // -------------- ASSERT --------------------
        Assert.AreEqual(expectedText, resultText);
    }


    // @todo Uncomment this when Flags2 are added to the EliteDangerousState
    //[Test]
    //public void StatusFlags2_Format_ForHumans()
    //{
    //    // -------------- ARRANGE -----------------------
    //    EDStatusFlags2 statusFlags = EDStatusFlags2.OnFoot | EDStatusFlags2.LowOxygen | EDStatusFlags2.VeryCold;
    //    string expectedText = "OnFoot | LowOxygen | VeryCold";

    //    // -------------- ACT -----------------------
    //    string resultText = statusViewController.FormatStatusFlagsForHumans<EDStatusFlags2>(statusFlags);

    //    // -------------- ASSERT --------------------
    //    Assert.AreEqual(expectedText, resultText);
    //}


    [Test]
    public void StatusChange_Updates_ProcessValues()
    {
        // -------------- ARRANGE -----------------------
        // Make sure we're starting with a clean slate for the eliteDangerousState
        Assert.AreEqual(eliteDangerousState.running, false);
        Assert.AreEqual(eliteDangerousState.processName, null);
        Assert.AreEqual(eliteDangerousState.processId, 0);
        Assert.AreEqual(eliteDangerousState.processDirectory, null);
        // Save the starting value from the Desktop UI element
        string startProcessActiveText = processActive.text;
        string startProcessNameText = processName.text;
        string startProcessIdText = processId.text;
        string startProcessDirText = processDir.text;

        #region -------------- ACT -----------------------
        string processActiveTestString = "True";
        string processNameTestString = "EliteTest64.exe";
        string processIdTestString = "12345";
        string processDirTestString = @"C:\Awesome\Saved Games\Frontier";
        // Change the value of the eliteDangerousState
        eliteDangerousState.running = true;
        eliteDangerousState.processName = processNameTestString;
        eliteDangerousState.processId = (uint)12345;
        eliteDangerousState.processDirectory = processDirTestString;
        // Initiate a refresh, which would normally be handled by a Listener component
        statusViewController.Refresh();

        #endregion

        // --------------- ASSERT --------------------
        string endProcessActiveText = processActive.text;
        string endProcessNameText = processName.text;
        string endProcessIdText = processId.text;
        string endProcessDirText = processDir.text;
        Assert.AreEqual(processActiveTestString, endProcessActiveText);
        Assert.AreEqual(processNameTestString, endProcessNameText);
        Assert.AreEqual(processIdTestString, endProcessIdText);
        Assert.AreEqual(processDirTestString, endProcessDirText);
    }
}
