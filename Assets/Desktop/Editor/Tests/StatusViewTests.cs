using UnityEngine;
using UnityEngine.UIElements;
using NUnit.Framework;
using EVRC.Desktop;
using System.Reflection;
using EVRC.Core;
using UnityEngine.TestTools;

public class StatusViewTests
{
    private VisualElement visualElement;
    private Label label;

    [SetUp]
    public void Setup()
    {
        // Create a VisualElement to attach the label
        visualElement = new VisualElement();

        // Add a Label object with the name "test-label"
        var labelObject = new Label("Test Label")
        {
            name = "test-label"
        };
        visualElement.Add(labelObject);

        // Query for the Label object with the name "test-label"
        label = visualElement.Query<Label>("test-label");

    }

    private GameObject CorrectlyBuildStatusView()
    {
        // Create a new GameObject 
        GameObject gameObject = new GameObject("Parent");
        UIDocument uIDocument = gameObject.AddComponent<UIDocument>();
        uIDocument.rootVisualElement.Add(visualElement);

        // Set parent of the child object
        GameObject childObject = new GameObject("Child");
        childObject.transform.SetParent(gameObject.transform);
        StatusView statusView = childObject.AddComponent<StatusView>();
        statusView.uiDocument = uIDocument;

        // Set the statusLabel element
        Label statusLabel = new Label();
        statusView._statusLabel = statusLabel;
        visualElement.Add(statusLabel);

        // This could be any derived version of GameState. Just needs an override of the GetStatusText method (which is abstract in GameStateBase).
        EliteDangerousState gameState = ScriptableObject.CreateInstance<EliteDangerousState>();
        statusView.gameState = gameState;

        return gameObject;
    }

    [Test]
    public void StatusViewComponent_BuildsCorrectly()
    {
        // Basically, can we enable it without errors

        GameObject gameObject = CorrectlyBuildStatusView();
        StatusView statusView = gameObject.GetComponentInChildren<StatusView>();
        statusView.OnEnable();

        // No Warning Messages
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void TestStatusViewUIDocumentInParentWarning()
    {
        // First build it correctly
        GameObject gameObject = CorrectlyBuildStatusView();
        StatusView statusView = gameObject.GetComponentInChildren<StatusView>();

        // Then null the uiDocument, which should throw an error
        statusView.uiDocument = null;
        statusView.OnEnable();

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(".*UIDocument.*"));
    }

    [Test]
    public void TestVJoyLabelTextUpdates()
    {
        // Save the starting value
        string startText = label.text;

        // Create a GameState object. This object is defined to have a single response to the expected method
        VJoyState gameState = ScriptableObject.CreateInstance<VJoyState>();
        gameState.vJoyStatus = VJoyStatus.NotInstalled;

        // Create a new StatusView Instance
        GameObject gameObject = CorrectlyBuildStatusView();
        StatusView statusView = gameObject.GetComponentInChildren<StatusView>();

        // Set the GameState to our test object
        statusView.gameState = gameState;

        // Use reflection to access and modify the private field.
        var myIntFieldInfo = typeof(StatusView).GetField("_statusLabel", BindingFlags.NonPublic | BindingFlags.Instance);
        myIntFieldInfo.SetValue(statusView, label);

        // Refresh function calls GetStatusText on whatever GameObject is attached
        // and assigns the value to the Label object
        statusView.Refresh();

        // Not the same as it was when we started
        Assert.AreNotEqual(label.text, startText);
        // Expected text 
        Assert.AreEqual(label.text, "Not installed");
    }
}