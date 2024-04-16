using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts string so it's easier to read
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string NicifyString(this string name)
        {
            // Split the input name by camel case and whitespace
            string[] words = Regex.Split(name, @"(?<!^)(?=[A-Z])|(?<=[a-z])(?=[A-Z])|[\s_-]+");

            // Join the words with spaces and capitalize the first letter of each word
            string nicifiedName = "";
            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0)
                {
                    nicifiedName += " ";
                }
                nicifiedName += char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            return nicifiedName;
        }

        /// <summary>
        /// Converts bytes to the largest possible byte string such as kilobyte and gigabyte
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static string ConvertToLargestBytesString(long bytes, int decimalPlaces = 1)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            decimalPlaces = Mathf.Clamp(decimalPlaces, 0, int.MaxValue);
            if (bytes < 0)
                return "-" + ConvertToLargestBytesString(-bytes, decimalPlaces);
            if (bytes == 0)
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(bytes, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)bytes / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}