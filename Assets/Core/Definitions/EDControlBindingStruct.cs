using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC.Core
{
    public struct ControlButtonBinding
    {
        public struct KeyModifier
        {
            public string Device;
            public string Key;

            public override bool Equals(object obj)
            {
                if (!(obj is KeyModifier))
                    return false;

                KeyModifier other = (KeyModifier)obj;
                return Device == other.Device && Key == other.Key;
            }

            public override int GetHashCode()
            {
                return (Device?.GetHashCode() ?? 0) ^ (Key?.GetHashCode() ?? 0);
            }
        }

        public struct KeyBinding
        {
            public string Device;
            public string Key;
            public string DeviceIndex;
            public HashSet<KeyModifier> Modifiers;

            public override bool Equals(object obj)
            {
                if (!(obj is KeyBinding))
                    return false;

                KeyBinding other = (KeyBinding)obj;
                return Device == other.Device &&
                       Key == other.Key &&
                       DeviceIndex == other.DeviceIndex &&
                       Modifiers.SetEquals(other.Modifiers);
            }

            public override int GetHashCode()
            {
                int hash = 17;
                hash = hash * 23 + (Device?.GetHashCode() ?? 0);
                hash = hash * 23 + (Key?.GetHashCode() ?? 0);
                hash = hash * 23 + (DeviceIndex?.GetHashCode() ?? 0);
                hash = hash * 23 + Modifiers.Aggregate(0, (acc, modifier) => acc + modifier.GetHashCode());
                return hash;
            }

            public string unityEditorString
            {
                get { return GetKeyBindingString(); }
            }

            // Method to return a string representation of the KeyBinding
            public string GetKeyBindingString()
            {
                if (Modifiers == null || Modifiers.Count == 0)
                {
                    return Key;
                }

                var modifiersString = string.Join(", ", Modifiers.Select(m => m.Key));
                return $"{Key} + {modifiersString}";
            }

            public bool IsValid
            {
                get { return Device != "{NoDevice}"; }
            }

            // Is this a Keyboard key press we can act on?
            public bool IsValidKeypress
            {
                get
                {
                    // Is it on the Keyboard device?
                    if (Device != "Keyboard") return false;
                    if (Key == string.Empty) return false;

                    foreach (var modifier in Modifiers)
                    {
                        if (modifier.Device != "Keyboard") return false;
                    }

                    return true;
                }
            }

            // Is there a VJoy action we can act on?
            public bool IsValidVJoyPress
            {
                get
                {
                    // Is it on the vJoy device?
                    if (Device != "vJoy") return false;
                    if (Modifiers.Count > 0) return false;
                    return true;
                }
            }
        }

        public KeyBinding Primary;
        public KeyBinding Secondary;

        public bool HasKeyboardKeybinding
        {
            get
            {
                return Primary.IsValidKeypress || Secondary.IsValidKeypress;
            }
        }

        public bool HasVJoyKeybinding
        {
            get
            {
                return Primary.IsValidVJoyPress || Secondary.IsValidVJoyPress;
            }
        }

        public KeyBinding? KeyboardKeybinding
        {
            get
            {
                if (Primary.IsValidKeypress) return Primary;
                if (Secondary.IsValidKeypress) return Secondary;
                return null;
            }
        }

        public KeyBinding? VJoyKeybinding
        {
            get
            {
                if (Primary.IsValidVJoyPress) return Primary;
                if (Secondary.IsValidVJoyPress) return Secondary;
                return null;
            }
        }
    }

}
