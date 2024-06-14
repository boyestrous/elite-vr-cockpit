using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TestUtils
{
    public static ControlButtonBinding.KeyBinding CreateKeyBinding(string key, List<string> modifiers, string device = "Keyboard")
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
}