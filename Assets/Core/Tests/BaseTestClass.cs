using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Setup and TearDown functions so you don't have to keep rewriting them
/// </summary>
public class BaseTestClass
{
    public Scene newScene;
    public GameObject gameObject;


    /// <summary>
    /// Creates a new scene (so nothing in the current scene interferes) and a new gameObject called "gameObject".
    /// </summary>
    public virtual void SetUp()
    {
        newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        // Set the newly created scene as the active scene
        SceneManager.SetActiveScene(newScene);

        gameObject = new GameObject("TestObject");
    }


    /// <summary>
    /// Removes all files and directories from the temporaryCachePath
    /// </summary>
    public virtual void TearDown()
    {
        DeleteFilesAndDirectories(Application.temporaryCachePath);
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
}
