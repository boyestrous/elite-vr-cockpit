using System;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using EVRC.Core;

namespace OverlayTests
{
    public class ValidFileTests : MonoBehaviour
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
            var tempState = new OverlayState()
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
            int expectedFileVersion = OverlayManager.currentFileVersion;

            // Act
            OverlayState overlayState = OverlayFileUtils.LoadFromFile(validTempStatePath);

            // Assert
            Assert.IsNotNull(overlayState);
            Assert.AreEqual(OverlayManager.currentFileVersion, overlayState.version);

            SavedGameObject[] staticLocations = overlayState.staticLocations;
            Assert.IsNotNull(staticLocations);
            Assert.AreEqual(1, staticLocations.Length);
            Assert.AreEqual("testObject", staticLocations[0].key);
            Assert.AreEqual(Vector3.one, staticLocations[0].overlayTransform.pos);
            Assert.AreEqual(Vector3.zero, staticLocations[0].overlayTransform.rot);

            SavedControlButton[] controlButtons = overlayState.controlButtons;
            Assert.IsNotNull(controlButtons);
            Assert.AreEqual(1, controlButtons.Length);
            Assert.AreEqual("testControlButton", controlButtons[0].type);
            Assert.AreEqual(Vector3.one, controlButtons[0].overlayTransform.pos);
            Assert.AreEqual(Vector3.zero, controlButtons[0].overlayTransform.rot);

            SavedBooleanSetting[] booleanSettings = overlayState.booleanSettings;
            Assert.IsNotNull(booleanSettings);
            Assert.AreEqual(1, booleanSettings.Length);
            Assert.AreEqual("testSetting", booleanSettings[0].name);
            Assert.AreEqual(true, booleanSettings[0].value);
        }

        [Test]
        public void FileNotFound_Returns_NewOverlayStateObject()
        {
            // Arrange
            string invalidPath = "--invalidPath--";

            // Act
            OverlayState overlayState = OverlayFileUtils.LoadFromFile(invalidPath);

            // Assert
            Assert.AreEqual(overlayState, new OverlayState());
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(validTempStatePath);
        }
    }
}