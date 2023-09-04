using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Utils
{
    public class EnumUtils : MonoBehaviour
    {
        /// <summary>
        /// Parses an Enum from a string and handles null/empty string values by returning default(Enum), which is the equivalent of an enum that has not been set. 
        /// </summary>
        /// <remarks>
        /// This is relevant when an enum is exposed in the inspector, but you have not selected a value. It may show "nothing" in the inspector window, even if there isn't
        /// a corresponding value in that enum. It also may select the first value of the enum, which corresponds to zero.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>default(T) if the string cannot be parsed, enum value otherwise</returns>
        public static T ParseEnumOrDefault<T>(string value) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value) || value.Equals("nothing", StringComparison.OrdinalIgnoreCase))
                return default(T);

            return Enum.TryParse(value, out T result) ? result : default(T);
        }

        /// <summary>
        /// Parses an Enum from a string and handles null/empty string values by returning the passed default value.
        /// </summary>
        /// <remarks>
        /// This is relevant when an enum is exposed in the inspector, but you have not selected a value. It may show "nothing" in the inspector window, even if there isn't
        /// a corresponding value in that enum. It also may select the first value of the enum, which corresponds to zero.
        /// </remarks>
        public static (bool, T1, T2) ParseEnumsOrDefault<T1,T2>(string value) where T1 : struct, Enum where T2 : struct, Enum
        {
            if (string.IsNullOrEmpty(value) || value.Equals("nothing", StringComparison.OrdinalIgnoreCase))
                return (false, default(T1), default(T2));

            if (Enum.TryParse(value, out T1 result))
            {
                return (true, result, default(T2));
            }
            else if (Enum.TryParse(value, out T2 result2))
            {
                return (true, default(T1), result2);
            }
            else
            {
                return (false, default(T1), default(T2));
            }
        }

        /// <summary>
        /// Allows you to set a custom default value that will return if an empty string is encountered while parsing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultOverride"></param>
        /// <returns></returns>
        public static T ParseEnumOrDefault<T>(string value, T defaultOverride) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value) || value.Equals("nothing", StringComparison.OrdinalIgnoreCase))
                return defaultOverride;

            return Enum.TryParse(value, out T result) ? result : default(T);
        }

    }
}
