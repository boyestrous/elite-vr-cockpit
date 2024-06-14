using EVRC.Desktop;
using NUnit.Framework;
using System.Collections;
using EVRC.Core.Tests;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using System.IO;
using System.Security.Cryptography;
using EVRC.Core;

public class LogListControllerTests : BaseTestClass
{
    ListView listView;
    List<LogItem> logItems;
    LogListController logListController;
    VisualTreeAsset listItemAsset;

    //Scene newScene;
    //GameObject gameObject;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        // Set the newly created scene as the active scene
        SceneManager.SetActiveScene(newScene);

        gameObject = new GameObject("TestObject");
        logListController = gameObject.AddComponent<LogListController>();
        listItemAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
        
        listView = new ListView();
        logItems = new List<LogItem>();

        logListController.m_AllLogs = logItems;
        logListController.m_LogList = listView;
        logListController.m_ListEntryTemplate = listItemAsset;
        logListController.SetListBindingMethods();

        Application.logMessageReceived += logListController.OnLogMessage;
    }

    [Test]
    public void ApplicationLogs_AddedTo_LogListSource() 
    {
        // Starts empty
        Assert.AreEqual(0, logListController.m_AllLogs.Count, "The Log List was NOT Empty at the beginning");

        Debug.Log("Test Log");

        // Ensure the underlying list has one log
        Assert.AreEqual(1, logListController.m_AllLogs.Count, "Log list did not update as expected.");

        // Ensure the ListView's itemsSource has one item
        Assert.AreEqual(1, listView.itemsSource.Count, "ListView itemsSource did not update as expected.");
    }

    [Test]
    public void LogList_Matches_Export() 
    {
        string messageOne = "Test Log 1";
        Debug.Log(messageOne);

        string messageTwo = "Test Log 2";
        Debug.LogWarning(messageTwo);

        string messageThree = "Test Log 3";
        Debug.LogError(messageThree);

        // Unity gets mad if you log an error without expecting it, but we need to ensure the errors get exported to file...
        LogAssert.Expect(LogType.Error, messageThree); 


        // The Export method appends the log type when writing to file
        messageOne = "Log: " + messageOne; 
        messageTwo = "Warning: " + messageTwo; 
        messageThree = "Error: " + messageThree; 

        string exportPath = Path.Combine(Application.temporaryCachePath, "export_test.log");

        logListController.ExportAllLogs(exportPath);

        // Ensure the file was created
        FileAssert.Exists(exportPath);

        // Read the file content
        string[] lines = File.ReadAllLines(exportPath);

        // Verify the number of lines
        Assert.AreEqual(6, lines.Length, "The exported log file should contain exactly 6 lines. The First three are header lines");

        // Verify the content of each line (the first three are headers)
        Assert.AreEqual(messageOne, lines[3], "The first log entry should match");
        Assert.AreEqual(messageTwo, lines[4], "The second log entry should match");
        Assert.AreEqual(messageThree, lines[5], "The third log entry should match");
    }

    [Test]
    public void Exported_Logs_DoNotTruncateWithMaxlines()
    {
        // Arrange
        //string exportPath = Path.Combine(Application.temporaryCachePath, "export_test_2.log");
        string exportPath = Paths.ExportedLogFileName;

        // Set the maxLines to 10. This is normally for managing performance in the ListView, but we need to ensure that the lines that are dropped from the ListView are still exported to the file.
        logListController.maxLines = 10;

        string firstMessage = "First Log Message";
        Debug.Log(firstMessage);

        for (int i = 0; i < 25; i++)
        {
            Debug.Log("Hello");
        }

        string lastMessage = "Final Log Message";
        Debug.Log(lastMessage);

        // The Export method appends the log type when writing to file
        firstMessage = "Log: " + firstMessage;
        lastMessage = "Log: " + lastMessage;

        // Act
        logListController.ExportAllLogs(exportPath);

        // Assert
        FileAssert.Exists(exportPath);

        // Read the file content
        string[] lines = File.ReadAllLines(exportPath);

        // Verify the content of each line
        Assert.AreEqual(firstMessage, lines[3], "The first log entry may have been truncated.");
        Assert.AreEqual(lastMessage, lines[^1], "The final log entry should match.");
    }
}

