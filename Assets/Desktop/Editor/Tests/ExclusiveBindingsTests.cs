using EVRC.Core;
using EVRC.Desktop;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExclusiveBindingsTests
{
    ExclusiveBindingSet exclusiveSetOne;
    List<EDControlButton> buttonsOne;
    List<EDControlButton> buttonsTwo;

    ControlButtonBinding.KeyBinding keyE_Binding;
    ControlButtonBinding.KeyBinding keyV_Binding;
    ControlButtonBinding.KeyBinding keyR_Binding;
    ControlButtonBinding.KeyBinding keyC_Binding;
    ControlButtonBinding.KeyBinding keyM_Binding;
    ControlButtonBinding.KeyBinding keyO_Binding;
    ControlButtonBinding.KeyBinding keyD_Binding;
    ControlButtonBinding.KeyBinding keyS_Binding;
    ControlButtonBinding.KeyBinding emptyBinding;
    GameObject go;
    ExclusiveBindings exclusiveBindingsController;

    [SetUp]
    public void SetUp()
    {
        // Create an Exclusive Set
        exclusiveSetOne = new ExclusiveBindingSet();

        buttonsOne = new List<EDControlButton>()
            {
                EDControlButton.AutoBreakBuggyButton,
                EDControlButton.BackwardThrustButton,
                EDControlButton.ForwardThrustButton,
                EDControlButton.CamZoomIn
            };
        buttonsTwo = new List<EDControlButton>()
            {
                EDControlButton.BuggyCycleFireGroupNext,
                EDControlButton.CamTranslateLeft,
                EDControlButton.YawCameraLeft,
                EDControlButton.LandingGearToggle
            };
        exclusiveSetOne.listOne = buttonsOne;
        exclusiveSetOne.listTwo = buttonsTwo;

        // Bindings
        keyE_Binding = TestUtils.CreateKeyBinding("Key_E", new List<string>());
        keyV_Binding = TestUtils.CreateKeyBinding("Key_V", new List<string>());
        keyR_Binding = TestUtils.CreateKeyBinding("Key_R", new List<string>() { "Key_LeftAlt" });
        keyC_Binding = TestUtils.CreateKeyBinding("Key_C", new List<string>() { "Key_LeftAlt", "Key_LeftShift" });
        keyM_Binding = TestUtils.CreateKeyBinding("Key_M", new List<string>());
        keyO_Binding = TestUtils.CreateKeyBinding("Key_O", new List<string>());
        keyD_Binding = TestUtils.CreateKeyBinding("Key_D", new List<string>());
        keyS_Binding = TestUtils.CreateKeyBinding("Key_S", new List<string>());
        // Empty binding for secondary keys that don't have values
        emptyBinding = TestUtils.CreateKeyBinding("", new List<string>());

        // Create the OverlappingBindings component
        go = new GameObject("Overlap Finder");
        exclusiveBindingsController = go.AddComponent<ExclusiveBindings>();

    }

    [Test]
    public void Identifies_Hardcoded_BindingOverlap()
    {
        #region ---- Arrange Region       
        // Common types of overlaps
        Dictionary<EDControlButton, ControlButtonBinding> buttonBindings = new Dictionary<EDControlButton, ControlButtonBinding>() 
        {
            // both with the same binding in the Primary position
            {EDControlButton.AutoBreakBuggyButton, new ControlButtonBinding() { Primary = keyE_Binding, Secondary = TestUtils.CreateKeyBinding("", new List<string>())} },
            {EDControlButton.BuggyCycleFireGroupNext, new ControlButtonBinding() { Primary = keyE_Binding, Secondary = TestUtils.CreateKeyBinding("", new List<string>())} },

            // both with the same binding in the Secondary position
            {EDControlButton.BackwardThrustButton, new ControlButtonBinding() { Secondary = keyV_Binding, Primary = TestUtils.CreateKeyBinding("", new List<string>())} },
            {EDControlButton.YawCameraLeft, new ControlButtonBinding() { Secondary = keyV_Binding, Primary = TestUtils.CreateKeyBinding("", new List<string>())} },

            // same binding in Primary / Secondary
            {EDControlButton.ForwardThrustButton, new ControlButtonBinding() { Primary = keyR_Binding, Secondary = TestUtils.CreateKeyBinding("", new List<string>())} },
            {EDControlButton.CamTranslateLeft, new ControlButtonBinding() { Secondary = keyR_Binding, Primary = TestUtils.CreateKeyBinding("", new List<string>())} },

            // same binding in Secondary / Primary
            {EDControlButton.CamZoomIn, new ControlButtonBinding() { Secondary = keyC_Binding, Primary = TestUtils.CreateKeyBinding("", new List<string>())} },
            {EDControlButton.LandingGearToggle, new ControlButtonBinding() { Primary = keyC_Binding, Secondary = TestUtils.CreateKeyBinding("", new List<string>())} },
        };


        // set the exclusive set lists, which would normally get set in the Unity Editor
        List<ExclusiveBindingSet> listOfExclusiveBindingSets = new List<ExclusiveBindingSet> { exclusiveSetOne };
        exclusiveBindingsController.exclusiveBindingSets = listOfExclusiveBindingSets;
        #endregion

        // Act - Run the find method
        List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)> foundProblems = exclusiveBindingsController.FindExclusiveBindingProblems(buttonBindings);

        #region --- Assert Region 
        // there are more than 0 overlapping bindings
        Assert.AreNotEqual(0, foundProblems.Count);
        Assert.AreEqual(foundProblems[0].Item1, EDControlButton.AutoBreakBuggyButton);
        Assert.AreEqual(foundProblems[0].Item2, EDControlButton.BuggyCycleFireGroupNext);
        Assert.IsTrue(foundProblems.Any(f => f.Item3.Equals(keyE_Binding)));
        Assert.IsTrue(foundProblems.Any(f => f.Item3.Equals(keyV_Binding)));
        Assert.IsTrue(foundProblems.Any(f => f.Item3.Equals(keyR_Binding)));
        Assert.IsTrue(foundProblems.Any(f => f.Item3.Equals(keyC_Binding)));
        #endregion
    }

    [Test]
    public void DoesNotReport_NonOverlappingBindings()
    {
        #region ---- Arrange Region
        // These should NOT cause problems
        Dictionary<EDControlButton, ControlButtonBinding> buttonBindings = new Dictionary<EDControlButton, ControlButtonBinding>()
        {
            // All of these are unique
            {EDControlButton.AutoBreakBuggyButton, new ControlButtonBinding() { Primary = keyE_Binding, Secondary = emptyBinding } },
            {EDControlButton.BuggyCycleFireGroupNext, new ControlButtonBinding() { Primary = keyV_Binding, Secondary = emptyBinding } },
            {EDControlButton.BackwardThrustButton, new ControlButtonBinding() { Secondary = keyR_Binding, Primary = emptyBinding } },
            {EDControlButton.YawCameraLeft, new ControlButtonBinding() { Secondary = keyC_Binding, Primary = emptyBinding } },
            {EDControlButton.ForwardThrustButton, new ControlButtonBinding() { Primary = keyM_Binding, Secondary = emptyBinding } },
            {EDControlButton.CamTranslateLeft, new ControlButtonBinding() { Secondary = keyO_Binding, Primary = emptyBinding } },
            {EDControlButton.CamZoomIn, new ControlButtonBinding() { Secondary = keyD_Binding, Primary = emptyBinding } },
            {EDControlButton.LandingGearToggle, new ControlButtonBinding() { Primary = keyS_Binding, Secondary = emptyBinding } },
        };

        // set the exclusive set lists, which would normally get set in the Unity Editor
        List<ExclusiveBindingSet> listOfExclusiveBindingSets = new List<ExclusiveBindingSet> { exclusiveSetOne };
        exclusiveBindingsController.exclusiveBindingSets = listOfExclusiveBindingSets;
        #endregion

        // Act - Run the find method
        List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)> foundProblems = exclusiveBindingsController.FindExclusiveBindingProblems(buttonBindings);

        #region --- Assert Region 
        // None of the bindings above should be problematic
        Assert.AreEqual(0, foundProblems.Count);
        #endregion
    }

    [Test]
    public void DoesNotReport_Overlaps_OutsideOfExclusiveSets()
    {
        #region ---- Arrange Region
        // These should NOT cause problems
        Dictionary<EDControlButton, ControlButtonBinding> buttonBindings = new Dictionary<EDControlButton, ControlButtonBinding>()
        {
            // This one is from the ExclusiveSet
            {EDControlButton.AutoBreakBuggyButton, new ControlButtonBinding() { Primary = keyE_Binding, Secondary = emptyBinding } },

            // This EDControlButton is NOT in the ExclusiveSet, so it can have an overlapping control
            {EDControlButton.DeployHardpointToggle, new ControlButtonBinding() { Primary = keyE_Binding, Secondary = emptyBinding } },
        };

        // set the exclusive set lists, which would normally get set in the Unity Editor
        List<ExclusiveBindingSet> listOfExclusiveBindingSets = new List<ExclusiveBindingSet> { exclusiveSetOne };
        exclusiveBindingsController.exclusiveBindingSets = listOfExclusiveBindingSets;
        #endregion

        // Act - Run the find method
        List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)> foundProblems = exclusiveBindingsController.FindExclusiveBindingProblems(buttonBindings);

        #region --- Assert Region 
        // None of the bindings above should be problematic
        Assert.AreEqual(0, foundProblems.Count);
        #endregion
    }

    [Test]
    public void TemplateFile_HasNoOverlaps()
    {
        // use the OnEnable element to access the default ExclusiveBindingSet
        exclusiveBindingsController.OnEnable();

        // Read the bindings from the template
        var buttonBindings = EDControlBindingsUtils.ParseFile(Paths.BindingsTemplatePath);

        // Act - Run the find method
        List<(EDControlButton, EDControlButton, ControlButtonBinding.KeyBinding)> foundProblems = exclusiveBindingsController.FindExclusiveBindingProblems(buttonBindings);

        #region --- Assert Region 
        // there are more than 0 overlapping bindings
        Assert.AreEqual(0, foundProblems.Count);
        #endregion
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

}
