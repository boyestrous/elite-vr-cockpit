using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

public class OverlayFileUtilsTests
{
    [SetUp]
    public void SetUp()
    {
        // Create a new empty scene
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        // Set the newly created scene as the active scene
        SceneManager.SetActiveScene(newScene);
    }

    // Method to generate random JSON content
    private static JObject GenerateRandomJson()
    {
        var json = new JObject();
        // Add random key-value pairs
        for (int i = 0; i < 5; i++)
        {
            string key = $"key{i}";
            string value = Guid.NewGuid().ToString();
            json.Add(key, value);
        }
        return json;
    }

    [Test]
    public void TemplateFile_Exists()
    {
        Assert.AreEqual(true, File.Exists(Paths.OverlayStateTemplatePath));
    }

    [Test]
    public void Load_TemplateFile_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => OverlayFileUtils.Load(Paths.OverlayStateTemplatePath));
    }

    [Test]
    public void Load_TemplateFile_IsValid()
    {
        SavedStateFile loadedFromTemplate = OverlayFileUtils.Load(Paths.OverlayStateTemplatePath);
        Assert.AreEqual(Paths.currentOverlayFileVersion, loadedFromTemplate.version);

        // Act & Assert
        foreach (PropertyInfo property in typeof(SavedStateFile).GetProperties())
        {
            var value = property.GetValue(loadedFromTemplate);
            Assert.IsNotNull(value, $"{property.Name} should not be null");
        }
    }

    /// <summary>
    /// Should not return any .json file, if that file doesn't have a "version" key. This should prevent any non-overlay files from being returned.
    /// </summary>
    [Test]
    public void GetAllSavedStateFiles_Skips_JSON_Without_Version()
    {
        // Create some extra json files that aren't SavedState files, they shouldn't be returned. Use a random number to make sure it stays valid as the overall file total.
        System.Random random = new System.Random();
        int randomNumber = random.Next(2, 5);

        for (int i = 1; i <= randomNumber; i++)
        {
            string fileName = $"random_{i}.json";
            string filePath = Path.Combine(Application.temporaryCachePath, fileName);
            JObject json = GenerateRandomJson();

            using (StreamWriter file = File.CreateText(filePath))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    json.WriteTo(writer);
                }
            }
            Console.WriteLine($"Generated file: {filePath}");
        }

        // Make twice as many copies of SavedState files 
        int numberOfSavedStateFiles = randomNumber * 2;
        for (int i = 0; i < numberOfSavedStateFiles; i++)
        {
            string tempDestinationPath = Path.Combine(Application.temporaryCachePath, $"TempSavedState{i}.json");
            File.Copy(Paths.OverlayStateTemplatePath, tempDestinationPath);
        }


        List<string> foundFiles = OverlayFileUtils.GetAllSavedStateFiles(Application.temporaryCachePath);
        Assert.AreEqual(numberOfSavedStateFiles, foundFiles.Count);
    }

    [Test]
    public void LoadFromFile_Loads_Default_IfRequestedIsMissing()
    {
        bool usedRandomFile = false;
        SavedStateFile randomDefaultFile;

        // if there isn't a default file available, then we need to generate one
        if (!File.Exists(Paths.OverlayStatePath))
        {
            usedRandomFile = true;
            // Make and place a file called SavedState.json in the temporaryCachePath
            randomDefaultFile = GenerateRandomSavedStateFile(); //Randomize the content so we don't accidentally hardcode a passing test

            // Write it to a file and make sure it's there
            OverlayFileUtils.WriteToFile(randomDefaultFile, Paths.OverlayStateFileName);
            Assert.IsTrue(File.Exists(Paths.OverlayStatePath));

            // Request loading of a filename that's not there
            SavedStateFile loadedFile = OverlayFileUtils.LoadFromFile("blahblah.json", Application.temporaryCachePath);

            // Assert that LoadFromFile uses the default file that was already in the folder
            Assert.AreEqual(randomDefaultFile.version, loadedFile.version);
            CollectionAssert.AreEqual(randomDefaultFile.controlButtons, loadedFile.controlButtons);
            CollectionAssert.AreEqual(randomDefaultFile.staticLocations, loadedFile.staticLocations);
            CollectionAssert.AreEqual(randomDefaultFile.booleanSettings, loadedFile.booleanSettings);


            // We created a random file for this test (in the actual application directory), remove it
            File.Delete(Paths.OverlayStatePath);
            Assert.IsFalse(File.Exists(Paths.OverlayStatePath)); // And then make sure it's gone!
        }
        else
        {
            // Request loading of a filename that's not there
            SavedStateFile loadedFile = OverlayFileUtils.LoadFromFile("blahblah.json", Application.temporaryCachePath);
            SavedStateFile actualDefaultFile = OverlayFileUtils.LoadFromFile(); // this overload just uses the default


            // Assert that LoadFromFile uses the default file that was already in the folder
            Assert.AreEqual(actualDefaultFile.version, loadedFile.version);
            CollectionAssert.AreEqual(actualDefaultFile.controlButtons, loadedFile.controlButtons);
            CollectionAssert.AreEqual(actualDefaultFile.staticLocations, loadedFile.staticLocations);
            CollectionAssert.AreEqual(actualDefaultFile.booleanSettings, loadedFile.booleanSettings);
        }
    }

    [Test]
    public void LoadFromFile_UsesTemplate_IfDefaultMissing()
    {
        string targetPath = Path.Combine(Application.temporaryCachePath, Paths.OverlayStateFileName);

        // Make sure there's not a file there already
        Assert.IsFalse(File.Exists(targetPath), $"Target file Must not exist for test to begin: {targetPath}. ");

        // Call the function asking for SavedState.json and the tempCachePath
        SavedStateFile loadedFile = OverlayFileUtils.LoadFromFile("SavedState.json", Application.temporaryCachePath);

        // Assert that a new file was created in the tempCachePath
        Assert.IsTrue(File.Exists(targetPath));

        // Assert that the new file matches the Template
        SavedStateFile templateFile = OverlayFileUtils.LoadFromFile(Paths.OverlayStateTemplatePath);
        Assert.AreEqual(templateFile.version, loadedFile.version);
        CollectionAssert.AreEqual(templateFile.controlButtons, loadedFile.controlButtons);
        CollectionAssert.AreEqual(templateFile.staticLocations, loadedFile.staticLocations);
        CollectionAssert.AreEqual(templateFile.booleanSettings, loadedFile.booleanSettings);
    }

    [TearDown]
    public void TearDown()
    {
        // Get the temporary cache path
        string cachePath = Application.temporaryCachePath;

        // Delete all files and directories within the cache path
        DeleteFilesAndDirectories(cachePath);

        // Optional: Verify that the cache path is empty after cleanup
        Assert.IsFalse(Directory.Exists(cachePath), "Temporary cache path is not empty after cleanup");
    }

    // Method to delete all files and directories recursively
    private void DeleteFilesAndDirectories(string path)
    {
        if (Directory.Exists(path))
        {
            foreach (string file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteFilesAndDirectories(directory);
            }

            // Delete the directory itself after all its contents are deleted
            Directory.Delete(path);
        }
    }

    private static SavedStateFile GenerateRandomSavedStateFile()
    {

        string[] BooleanSettingNames = { "Setting1", "Setting2", "Setting3", "Setting4", "Setting5" };
        string[] ControlButtonTypes = { "Type1", "Type2", "Type3", "Type4", "Type5" };
        string[] ControlButtonAnchorStatusFlags = { "Anchor1", "Anchor2", "Anchor3", "Anchor4", "Anchor5" };
        string[] ControlButtonAnchorGuiFocus = { "Focus1", "Focus2", "Focus3", "Focus4", "Focus5" };
        string[] GameObjectKeys = { "Key1", "Key2", "Key3", "Key4", "Key5" };
        System.Random random = new System.Random();

        int booleanSettingsCount = random.Next(2, 6); // 2-5 items
        int controlButtonsCount = random.Next(2, 6); // 2-5 items
        int staticLocationsCount = random.Next(2, 6); // 2-5 items

        SavedBooleanSetting[] booleanSettings = new SavedBooleanSetting[booleanSettingsCount];
        for (int i = 0; i < booleanSettingsCount; i++)
        {
            booleanSettings[i] = new SavedBooleanSetting
            {
                name = BooleanSettingNames[random.Next(BooleanSettingNames.Length)],
                value = random.Next(0, 2) == 0 // Random true or false
            };
        }

        SavedControlButton[] controlButtons = new SavedControlButton[controlButtonsCount];
        for (int i = 0; i < controlButtonsCount; i++)
        {
            controlButtons[i] = new SavedControlButton
            {
                type = ControlButtonTypes[random.Next(ControlButtonTypes.Length)],
                anchorStatusFlag = ControlButtonAnchorStatusFlags[random.Next(ControlButtonAnchorStatusFlags.Length)],
                anchorGuiFocus = ControlButtonAnchorGuiFocus[random.Next(ControlButtonAnchorGuiFocus.Length)],
                overlayTransform = GenerateRandomOverlayTransform()
            };
        }

        SavedGameObject[] staticLocations = new SavedGameObject[staticLocationsCount];
        for (int i = 0; i < staticLocationsCount; i++)
        {
            staticLocations[i] = new SavedGameObject
            {
                key = GameObjectKeys[random.Next(GameObjectKeys.Length)],
                overlayTransform = GenerateRandomOverlayTransform()
            };
        }

        return new SavedStateFile
        {
            version = 5,
            booleanSettings = booleanSettings,
            controlButtons = controlButtons,
            staticLocations = staticLocations
        };
    }

    private static OverlayTransform GenerateRandomOverlayTransform()
    {
        return new OverlayTransform
        {
            pos = new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f)),
            rot = new Vector3(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f))
        };
    }


}
