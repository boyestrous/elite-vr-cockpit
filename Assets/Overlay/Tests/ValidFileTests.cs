using System;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using EVRC.Core;

public class ValidFileTests
{
    private string validTempStatePath;

    /// <summary>
    /// Create a known valid SavedStateFile
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // path is typically: %AppData%/Local/Temp/Username/Elite VR Cockpit/
        validTempStatePath = Path.Combine(Application.temporaryCachePath, "ValidStateFile.json");
        var tempState = new SavedStateFile()
        {
            version = 5,
            staticLocations = new SavedGameObject[1]{ new SavedGameObject()
            {
                key="testObject",
                overlayTransform = new OverlayTransform()
                {
                    pos = Vector3.one,
                    rot = Vector3.zero
                }
            }},
            controlButtons = new SavedControlButton[1] { new SavedControlButton()
            {
                overlayTransform = new OverlayTransform()
                {
                    pos = Vector3.one,
                    rot = Vector3.zero
                },
                type = "testControlButton"
            }},
            booleanSettings = new SavedBooleanSetting[1]
            {
                new SavedBooleanSetting()
                {
                    name = "testSetting",
                    value = true
                }
            },
        };

        File.WriteAllText(validTempStatePath, JsonUtility.ToJson(tempState));
    }

    [Test]
    public void ValidFile_PopulatesOverlayStateObject()
    {
        int expectedFileVersion = Paths.currentOverlayFileVersion;

        // Act
        SavedStateFile savedState = OverlayFileUtils.LoadFromFile(validTempStatePath);

        // Assert
        Assert.IsNotNull(savedState);
        Assert.AreEqual(Paths.currentOverlayFileVersion, savedState.version);

        SavedGameObject[] staticLocations = savedState.staticLocations;
        Assert.IsNotNull(staticLocations);
        Assert.AreEqual(1, staticLocations.Length);
        Assert.AreEqual("testObject", staticLocations[0].key);
        Assert.AreEqual(Vector3.one, staticLocations[0].overlayTransform.pos);
        Assert.AreEqual(Vector3.zero, staticLocations[0].overlayTransform.rot);

        SavedControlButton[] controlButtons = savedState.controlButtons;
        Assert.IsNotNull(controlButtons);
        Assert.AreEqual(1, controlButtons.Length);
        Assert.AreEqual("testControlButton", controlButtons[0].type);
        Assert.AreEqual(Vector3.one, controlButtons[0].overlayTransform.pos);
        Assert.AreEqual(Vector3.zero, controlButtons[0].overlayTransform.rot);

        SavedBooleanSetting[] booleanSettings = savedState.booleanSettings;
        Assert.IsNotNull(booleanSettings);
        Assert.AreEqual(1, booleanSettings.Length);
        Assert.AreEqual("testSetting", booleanSettings[0].name);
        Assert.AreEqual(true, booleanSettings[0].value);
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(validTempStatePath);
    }
}
