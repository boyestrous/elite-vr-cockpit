using EVRC.Core;
using EVRC.Core.Overlay;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLocationsManagerTests
{
    private StaticLocationsManager staticLocationsManager;
    private SavedGameObject[] loadedGameObjects;
    GameObject objectOne;
    GameObject objectTwo; 
    GameObject objectThree;
    StaticLocationAnchor staticAnchor1;
    StaticLocationAnchor staticAnchor2;
    StaticLocationAnchor staticAnchor3;

    [SetUp]
    public void SetUp()
    {
        GameObject gameObject = new GameObject("Manager");
        staticLocationsManager = gameObject.AddComponent<StaticLocationsManager>();

        // Mimick a loaded array from the file. These are the objects 
        // we're trying to place in the scene from the file
        loadedGameObjects = new SavedGameObject[2];
        loadedGameObjects[0] = new SavedGameObject()
        {
            key = "firstTestObject",
            overlayTransform = new OverlayTransform()
            {
                pos = Vector3.zero,
                rot = Vector3.zero
            }
        };
        loadedGameObjects[1] = new SavedGameObject()
        {
            key = "secondTestObject",
            overlayTransform = new OverlayTransform()
            {
                pos = Vector3.one,
                rot = Vector3.one
            }
        };


        // These Anchors match the loadedGameObjects
        objectOne = new GameObject("One");
        objectTwo = new GameObject("Two");        
        staticAnchor1 = objectOne.AddComponent<StaticLocationAnchor>();
        staticAnchor2 = objectTwo.AddComponent<StaticLocationAnchor>();
        staticAnchor1.key = "firstTestObject";
        staticAnchor2.key = "secondTestObject";
        

        // This anchor was not present in the loadedGameObjects
        objectThree = new GameObject("Three");
        staticAnchor3 = objectThree.AddComponent<StaticLocationAnchor>();
        staticAnchor3.key = "thirdTestObject";


        //In a real scene the movable auto-assigns to its parent, but it's not
        //consistent in the testing enviornment
        staticAnchor1.movable.targetTransform = objectOne.transform;
        staticAnchor2.movable.targetTransform = objectTwo.transform;
        staticAnchor3.movable.targetTransform = objectThree.transform;


        // Enable the manager to get references of all of the anchors in the scene
        staticLocationsManager.OnEnable();

    }

    /// <summary>
    /// When a file is loaded, if the scene has new StaticLocationAnchors that aren't in the file, 
    /// it should NOT throw an error
    /// </summary>
    [Test]
    public void NewStaticLocations_MissingFromFile_ThrowsNoErrors()
    {      
        Assert.DoesNotThrow(() =>
        {
            staticLocationsManager.PlaceAll(loadedGameObjects);
        });
    }

    /// <summary>
    /// When a file is loaded, if the file has StaticLocationAnchors that aren't present in the scene 
    /// it should not throw an error
    /// </summary>
    [Test]
    public void Unexpected_StaticLocationAnchor_ThrowsError()
    {
        // Overwrite the first object with one that does not match a key for a StaticLocationAnchor
        // IN the current scene
        loadedGameObjects[0] = new SavedGameObject()
        {
            key = "unknownTestObject",
            overlayTransform = new OverlayTransform()
            {
                pos = Vector3.zero,
                rot = Vector3.zero
            }
        };

        Assert.Throws<KeyNotFoundException>(() =>
        {
            staticLocationsManager.PlaceAll(loadedGameObjects);
        });
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(objectOne);
        GameObject.DestroyImmediate(objectTwo);
        GameObject.DestroyImmediate(objectThree);
    }


}
