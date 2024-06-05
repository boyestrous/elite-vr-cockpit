using EVRC.Core;
using EVRC.Desktop;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class AvailableBindingsTests 
{ 

    // Test with a mini, hardcoded ControlBindingsState
    [Test]
    public void FindUnusedKeyBindings_With_HardcodedBindings()
    {
        ControlBindingsState state = ScriptableObject.CreateInstance<ControlBindingsState>();

        #region ---- Create some hardcoded keybindings ----
        // just a single key, no modifiers
        ControlButtonBinding.KeyBinding singleKeyBindingOne = CreateKeyBinding("Key_A", new List<string>());
        ControlButtonBinding.KeyBinding singleKeyBindingTwo = CreateKeyBinding("Key_B", new List<string>());
        ControlButtonBinding.KeyBinding singleKeyBindingThree = CreateKeyBinding("Key_Z", new List<string>());

        // one modifier (intentionally using the same single key)
        ControlButtonBinding.KeyBinding bindingWithModifierOne = CreateKeyBinding("Key_A", new List<string>() { "Key_LeftShift" });
        
        // one modifier with different key than singlekey
        ControlButtonBinding.KeyBinding bindingWithModifierTwo = CreateKeyBinding("Key_F", new List<string>() { "Key_LeftAlt" });

        // Blank or Empty binding (we still need to create blank entries for unused Keybindings or else the FindUnusedKeyBindings method will fail)
        // In reality, when reading from a file, we will never encounter a situation where the <Secondary> XML tag is missing. It's there with blank properties, which is what the binding below simulates...
        ControlButtonBinding.KeyBinding blankKeyBinding = CreateKeyBinding("", new List<string>(),"");


        // Create some bindings
        state.buttonBindings = new Dictionary<EDControlButton, ControlButtonBinding>()
        {
            // single key in Primary position
            { EDControlButton.AutoBreakBuggyButton, new ControlButtonBinding(){ Primary = singleKeyBindingOne, Secondary = blankKeyBinding} },
            // single key in Secondary position
            { EDControlButton.FireChaffLauncher, new ControlButtonBinding(){ Primary = blankKeyBinding, Secondary = singleKeyBindingTwo} },
            // single key with modifier in Primary position
            { EDControlButton.BackwardKey, new ControlButtonBinding(){ Primary = bindingWithModifierOne, Secondary = blankKeyBinding} },
            // single key with modifier in Secondary position
            { EDControlButton.CamPitchDown, new ControlButtonBinding(){ Primary = blankKeyBinding, Secondary = bindingWithModifierTwo} },
        };

        // Make sure they got created/added correctly
        Assert.IsTrue(state.buttonBindings.Count != 0);
        #endregion

        // Arrange
        GameObject go = new GameObject("TestGameObject");
        AvailableBindings availableBindings = go.AddComponent<AvailableBindings>();
        availableBindings.bindings = state;

        // Act
        List<ControlButtonBinding.KeyBinding> unused = availableBindings.FindUnusedKeyBindings();
        Assert.IsNotNull(unused);

        // Add a keybinding to the unused list to make sure the assertions would fail if there was a match
        unused.Add(singleKeyBindingThree);

        #region -- Assert Region --
        // unused SHOULD have the one we just added
        Assert.IsTrue(unused.Any(u => u.Equals(singleKeyBindingThree)));

        // unused list doesn't have matches for our samples
        Assert.IsFalse(unused.Any(u => u.Equals(singleKeyBindingOne)));
        Assert.IsFalse(unused.Any(u => u.Equals(singleKeyBindingTwo)));
        Assert.IsFalse(unused.Any(u => u.Equals(bindingWithModifierOne)));
        Assert.IsFalse(unused.Any(u => u.Equals(bindingWithModifierTwo)));
        #endregion
    }

    private ControlButtonBinding.KeyBinding CreateKeyBinding(string key, List<string> modifiers, string device = "Keyboard") 
    {
        ControlButtonBinding.KeyBinding binding = new ControlButtonBinding.KeyBinding();
        binding.Key = key;
        binding.Device = device;
        binding.Modifiers = new HashSet<ControlButtonBinding.KeyModifier>();

        for (int i = 0; i < modifiers.Count; i++)
        {
            ControlButtonBinding.KeyModifier tempModifier = new ControlButtonBinding.KeyModifier();
            tempModifier.Key = modifiers.ElementAt(i);
            tempModifier.Device = device;

            binding.Modifiers.Add(tempModifier);
        }

        return binding;
    }

    [Test]
    public void TemplateFileExists()
    {
        FileAssert.Exists(Paths.BindingsTemplatePath);
    }

    // Test with the current template file
    [Test]
    public void FindUnusedKeyBindings_with_TemplateFile()
    {
        // Arrange
        ControlBindingsState stateFromTemplate = ScriptableObject.CreateInstance<ControlBindingsState>();
        stateFromTemplate.buttonBindings = EDControlBindingsUtils.ParseFile(Paths.BindingsTemplatePath);
        Assert.IsNotNull(stateFromTemplate.buttonBindings);

        GameObject go = new GameObject("TestGameObject2");
        AvailableBindings availableBindings = go.AddComponent<AvailableBindings>();
        availableBindings.bindings = stateFromTemplate;

        // Act
        List<ControlButtonBinding.KeyBinding> unused = availableBindings.FindUnusedKeyBindings();
        Assert.IsNotNull(unused);

        #region --- Assert Region ---
        // Make a list of all valid keyboard bindings in the template file
        List<ControlButtonBinding.KeyBinding> validKeyboardBindings = stateFromTemplate.buttonBindings
            .Values // Get the collection of ControlButtonBinding values from the dictionary
            .Where(binding => binding.HasKeyboardKeybinding) // Filter for bindings with a keyboard keybinding
            .SelectMany(binding => new List<ControlButtonBinding.KeyBinding?> { binding.KeyboardKeybinding }) // Project each binding into an enumerable sequence of nullable KeyBinding
            .Where(keyBinding => keyBinding.HasValue) // Filter out the null values
            .Select(keyBinding => keyBinding.Value) // Select the actual KeyBinding value from the nullable
            .ToList(); // Convert to a list

        // Check each one
        foreach(var b in validKeyboardBindings)
        {
            Assert.IsFalse(unused.Any(u => u.Equals(b)));
        }

        #endregion
    }
}

