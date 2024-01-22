using System.Text.RegularExpressions;

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
    }
}