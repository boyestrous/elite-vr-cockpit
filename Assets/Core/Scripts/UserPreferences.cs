using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// USE SPARINGLY Most things should be stored in the SavedState system, which controls the overlay and other user settings. This class uses the Unity PlayerPrefs storage system to store information that's not good for the SavedState system (like the name of the preferred SavedState File itself...)
    /// <remarks>
    /// The PlayerPrefs should be used to save other information that is needed in situations where the SavedState may not be loaded. The SavedState keeps track of the overlay: placement of holographic elements, preferences like whether or not to display Labels on the buttons, and (eventually) color, so it may not be loaded (or exist at all) if the user is just navigating the desktop view.
    /// </remarks>
    /// </summary>
    public static class UserPreferences
    {
        // Set the string keys for the Key-Value pairs
        private const string LastUsedJsonFileKey = "LastUsedJsonFile";

        public static void SaveLastUsedJsonFile(string fileName)
        {
            PlayerPrefs.SetString(LastUsedJsonFileKey, fileName);
            PlayerPrefs.Save();
        }

        public static string GetLastUsedJsonFile()
        {
            if (PlayerPrefs.HasKey(LastUsedJsonFileKey))
            {
                return PlayerPrefs.GetString(LastUsedJsonFileKey);
            }
            return null;
        }
    }
}
