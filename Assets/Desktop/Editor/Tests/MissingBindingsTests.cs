using EVRC.Core.Actions;
using EVRC.Core;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EVRC.Desktop;
using System.IO;
using EVRC.Core.Overlay;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using System.Xml.Linq;
using UnityEditor;

public class MissingBindingsTests : MonoBehaviour
{
    public SavedGameState savedGameState;
    public ControlBindingsState bindingsState;
    public ControlButtonAssetCatalog assetCatalog;
    public ControlBindingsManager controlBindingsManager;

    //Component we're testing
    public MissingBindingsListController missingBindingsListController;
    public UIDocument uiDocument;

    private string tempBindsFilePath;


    [SetUp]
    public void SetUp()
    {
        tempBindsFilePath = Path.Combine(Application.temporaryCachePath, "Test.binds");

        // Create a missing bindings controller and the required state objects
        GameObject go = new GameObject("Active");
        missingBindingsListController = go.AddComponent<MissingBindingsListController>();
        savedGameState = ScriptableObject.CreateInstance<SavedGameState>();        
        bindingsState = ScriptableObject.CreateInstance<ControlBindingsState>();
        assetCatalog = ScriptableObject.CreateInstance<ControlButtonAssetCatalog>();
        assetCatalog.defaultTexture = new Texture2D(128,128);
        assetCatalog.defaultOffTexture = new Texture2D(128, 128);
        missingBindingsListController.savedGameState = savedGameState;
        missingBindingsListController.bindingsState = bindingsState;
        missingBindingsListController.assetCatalog = assetCatalog;

        // Construct the full path to the UXML file.
        string uxmlPath = "Assets/Desktop/BindingsView/BindingsContainer.uxml";

        // Load the UXML document.
        //VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

        //if (visualTreeAsset != null)
        //{
        //    // Clone the UXML content and bind it to the UIDocument.
        //    var clonedTree = visualTreeAsset.CloneTree();
        //    // Create a new UIDocument and bind it to the UXML content.
        //    uiDocument = new UIDocument();
        //    uiDocument.panelSettings.visualTree.Add(clonedTree);
        //}
        //else
        //{
        //    Debug.LogError("UXML file not found at path: " + uxmlPath);
        //}
        //missingBindingsListController.parentUIDocument = parentUIDoc;

        // Create a bindings manager and assign our bindingsState object
        GameObject cbm = new GameObject("BindingsManager");
        controlBindingsManager = cbm.AddComponent<ControlBindingsManager>();
        controlBindingsManager.controlBindingsState = bindingsState;

        missingBindingsListController.OnEnable();
    }


    /// <summary>
    /// If you have a holographic button in the scene, but the associated control doesn't have a keyboard
    /// or vJoy binding in the bindings file. This test ensures that the missing bindings can be identified.
    /// </summary>
    [Test]
    public void MissingBindings_Identified()
    {
        // Sample bindings file with 3 options
        // 1 - UI_Select has both Keyboard and vJoy Bindings
        // 2 - HeadlookPitchDown has neither Keyboard NOR vJoy bindings
        // 3 - VerticalThrustersDown has only vJoy binding
        // 4 - YawLeftButton has only Keyboard binding
        // 5 - RollLeftButton has neither Keyboard NOR vJoy bindings
        var bindingsString =
            @"<?xml version=""1.0"" encoding=""UTF-8"" ?>
            <Root PresetName=""Custom"" MajorVersion=""4"" MinorVersion=""0"">
                <UI_Select>
		            <Primary Device=""Keyboard"" Key=""Key_Space"" />
		            <Secondary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_1"" />
	            </UI_Select>
                <HeadLookPitchDown>
		            <Primary Device=""{NoDevice}"" Key="""" />
		            <Secondary Device=""{NoDevice}"" Key="""" />
	            </HeadLookPitchDown>
                <VerticalThrustersButton>
		            <Primary Device=""vJoy"" DeviceIndex=""0"" Key=""Joy_7"" />
		            <Secondary Device=""{NoDevice}"" Key="""" />
		            <ToggleOn Value=""0"" />
	            </VerticalThrustersButton>
                <YawLeftButton>
		            <Primary Device=""Keyboard"" Key=""Key_A"" />
		            <Secondary Device=""{NoDevice}"" Key="""" />
	            </YawLeftButton>
                <RollLeftButton>
		            <Primary Device=""{NoDevice}"" Key="""" />
		            <Secondary Device=""{NoDevice}"" Key="""" />
	            </RollLeftButton>
            </Root>";
        
        File.WriteAllText(tempBindsFilePath, bindingsString);

        // Override the default file path to use the testing version
        controlBindingsManager.SetBindingsFile(tempBindsFilePath);
        controlBindingsManager.LoadControlBindings();

        Assert.AreEqual(5, bindingsState.buttonBindings.Count);
        Assert.Contains(EDControlButton.UI_Select, bindingsState.buttonBindings.Keys);
        Assert.Contains(EDControlButton.HeadLookPitchDown, bindingsState.buttonBindings.Keys);
        Assert.Contains(EDControlButton.VerticalThrustersButton, bindingsState.buttonBindings.Keys);
        Assert.Contains(EDControlButton.YawLeftButton, bindingsState.buttonBindings.Keys);
        Assert.Contains(EDControlButton.RollLeftButton, bindingsState.buttonBindings.Keys);

        // Populate the controlButtons in SavedState
        // This is the list the Missing Validator uses to check against
        savedGameState.controlButtons = new List<SavedControlButton>()
        {
            new SavedControlButton(){ type = "UI_Select"},
            new SavedControlButton(){ type = "HeadLookPitchDown"},
            new SavedControlButton(){ type = "VerticalThrustersButton"},
            new SavedControlButton(){ type = "YawLeftButton"},
        };
        missingBindingsListController.savedGameState = savedGameState;
        missingBindingsListController.OnEnable();


        //Fill the assetCatalog with matching assets
        assetCatalog.controlButtons = new ControlButtonAsset[4];
        assetCatalog.controlButtons[0] = new SimpleControlButtonAsset()
        {
            name = "UI_Select",
            text = "ui_select",
            control = EDControlButton.UI_Select
        };
        assetCatalog.controlButtons[1] = new SimpleControlButtonAsset()
        {
            name = "HeadLookPitchDown",
            text = "HeadLookPitchDown",
            control = EDControlButton.HeadLookPitchDown
        };
        assetCatalog.controlButtons[2] = new SimpleControlButtonAsset()
        {
            name = "VerticalThrustersButton",
            control = EDControlButton.VerticalThrustersButton
        };
        assetCatalog.controlButtons[3] = new SimpleControlButtonAsset()
        {
            name = "YawLeftButton",
            control = EDControlButton.YawLeftButton
        };
        assetCatalog.OnEnable();


        // ----------------- Act ------------------------
        missingBindingsListController.ValidateBindings();

        // ----------------- Assert ------------------------
        // Should only be missing the HeadLookPitchDown because all of the others have a binding
        Assert.AreEqual(1, missingBindingsListController.missingBindings.Count);
        
    }
}
