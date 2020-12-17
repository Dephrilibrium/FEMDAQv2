using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMDAQ.StaticHelper
{
    public class StringHelper
    {
        /// <summary>
        /// Trims all symbols which means "empty" like space and tab from the given string.
        /// 
        /// Returns null if the given string doesn't exist.
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        static public string TrimString(string @string)
        {
            if (@string == null)
                return null;                // Error: Can't trim non-existing string.

            @string = @string.Trim();

            return @string;            // No complications!
        }



        /// <summary>
        /// Trims all symbols which means "empty" like space and tab from every string in the given string-array.
        /// 
        /// Returns null if the given array doesn't exists. Not existing elements of the array are skipped.
        /// /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] TrimArray(string[] array)
        {
            if (array == null)
                return null;                // Error: Can't trim null-pointer

            var trimmedArray = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
                trimmedArray[i] = TrimString(array[i]);

            return trimmedArray;             // No complications!
        }



        public static List<string> TrimList(List<string> list)
        {
            if (list == null)
                return null;

            var trimmedList = new List<string>();
            foreach (var @string in list)
                trimmedList.Add(TrimString(@string));

            return trimmedList;
        }



        /// <summary>
        /// This function is looking for a string in an array which starts with the given value and returns the found string.
        /// 
        /// You will get back the first found string as result. On an appearing error you get back null. You also will get null if there is no string found.
        /// </summary>
        /// <param name="stringArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public string FindStringWhichStartsWith(IEnumerable<string> stringArray, string value)
        {
            if (stringArray == null || value == null)
                return null;                // Error: Array or Value doesn't exist

            foreach (var line in stringArray)
            {
                if (line == null)
                    continue;

                if (line.StartsWith(value))
                    return line;
            }

            return null; // Not found
        }



        /// <summary>
        /// Converts a given double-array into a stringarray.
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        static public string[] ConvertDoubleArrayToStringArray(double[] Array)
        {
            if (Array == null)
                return null; // Error: No array given!
            string[] stringArray = new string[Array.Length];

            for (int arrayIndex = 0; arrayIndex < Array.Length; arrayIndex++)
                stringArray[arrayIndex] = Array[arrayIndex].ToString();

            return stringArray;
        }
    }
}
