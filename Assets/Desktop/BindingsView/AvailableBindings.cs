using EVRC.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Desktop
{
    public class AvailableBindings : MonoBehaviour
    {
        public ControlBindingsState bindings;

        public List<string> availableKeyBindings;

        public List<ControlButtonBinding.KeyBinding> unusedKeyBindings;
        HashSet<(string Key, HashSet<string> Modifiers)> usedKeyBindings;

        // All valid strings from the KeyboardInterface
        private List<string> allKeyStrings;

        // These key strings are not great for consistent implementation, they are typically modifiers or special keys.
        private static List<string> ignoreKeyStrings = new List<string> {
                "Key_Escape",
                "Key_LeftShift",
                "Key_LeftControl",
                "Key_LeftAlt",
                "Key_RightShift",
                "Key_RightControl",
                "Key_RightAlt",
                "Key_Backspace",
                "Key_Tab",
                "Key_Enter",
                "Key_CapsLock",
                "Key_Delete",
                "Key_Space"
            };

        public List<ControlButtonBinding.KeyBinding> FindUnusedKeyBindings()
        {
            availableKeyBindings = new List<string>();
            usedKeyBindings = new HashSet<(string Key, HashSet<string> Modifiers)>();
            unusedKeyBindings = new List<ControlButtonBinding.KeyBinding>();

            // Get all available key strings that could be used 
            allKeyStrings = KeyboardInterface.GetAllKeycodeStrings();

            // Filter out the ignored keys
            allKeyStrings = allKeyStrings.Where(item => !ignoreKeyStrings.Contains(item)).ToList();

            HashSet<string> modKeysToCheck = new HashSet<string>()
            {
                "Key_LeftShift",
                "Key_LeftControl",
                "Key_LeftAlt",
            };

            var bindValues = bindings.buttonBindings.Values;


            // Collect all used key bindings
            foreach (var binding in bindValues)
            {
                var primaryModifiers = new HashSet<string>(binding.Primary.Modifiers.Select(m => m.Key));
                usedKeyBindings.Add((binding.Primary.Key, primaryModifiers));

                var secondaryModifiers = new HashSet<string>(binding.Secondary.Modifiers.Select(m => m.Key));
                usedKeyBindings.Add((binding.Secondary.Key, secondaryModifiers));
            }

            // Generate all possible combinations of keys and modifiers
            foreach (var key in allKeyStrings)
            {

                // Check the key without any modifiers
                var emptyModifiersSet = new HashSet<ControlButtonBinding.KeyModifier>();
                var emptyModifiersKeys = new HashSet<string>();

                if (!usedKeyBindings.Contains((key, emptyModifiersKeys)))
                {
                    var keyBindingWithoutModifiers = new ControlButtonBinding.KeyBinding
                    {
                        Device = "Keyboard",
                        Key = key,
                        DeviceIndex = "0",
                        Modifiers = emptyModifiersSet
                    };
                    unusedKeyBindings.Add(keyBindingWithoutModifiers);
                }

                foreach (var modifier in modKeysToCheck)
                {
                    var modifiersSet = new HashSet<ControlButtonBinding.KeyModifier>
                {
                    new ControlButtonBinding.KeyModifier { Device = "Keyboard", Key = modifier }
                };

                    var keyBinding = new ControlButtonBinding.KeyBinding
                    {
                        Device = "Keyboard",
                        Key = key,
                        DeviceIndex = "0",
                        Modifiers = modifiersSet
                    };

                    var modifiersKeys = new HashSet<string>(modifiersSet.Select(m => m.Key));

                    if (!usedKeyBindings.Contains((key, modifiersKeys)))
                    {
                        unusedKeyBindings.Add(keyBinding);
                    }
                }
            }

            unusedKeyBindings.ForEach(x => availableKeyBindings.Add(x.unityEditorString));
            return unusedKeyBindings;
        }
    }
}
