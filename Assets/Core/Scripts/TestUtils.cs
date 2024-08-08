using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class TestUtils
{
    public static ControlButtonBinding.KeyBinding UnboundKeyBinding => CreateKeyBinding("", new List<string>(), device: "{NoDevice}");

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

    public static void DeleteFilesAndDirectories(string path)
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