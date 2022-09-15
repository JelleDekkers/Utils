using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this T[] arr)
        {
            return arr[Random.Range(0, arr.Length)];
        }

        public static T GetRandom<T>(this List<T> arr)
        {
            return arr[Random.Range(0, arr.Count)];
        }

        /// <summary>
        /// Returns a random value, except for excludeIndex"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="excludeIndex"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this T[] arr, int excludeIndex)
        {
            int index = (excludeIndex + Random.Range(1, arr.Length - 1)) % arr.Length;
            return arr[index];
        }

        /// <summary>
        /// Returns a random value, except for excludeIndex"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="excludeIndex"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this List<T> arr, int excludeIndex)
        {
            int index = (excludeIndex + Random.Range(1, arr.Count - 1)) % arr.Count;
            return arr[index];
        }

        public static void ReorderItem(this IList collection, int currentIndex, int desiredIndex)
        {
            if (desiredIndex < 0)
            {
                throw new System.Exception(string.Format("DesiredIndex {0} is smaller than zero", desiredIndex));
            }
            else if (desiredIndex > collection.Count - 1)
            {
                throw new System.Exception(string.Format("DesiredIndex {0} is larger than collection range", desiredIndex));
            }

            object item = collection[currentIndex];
            collection.RemoveAt(currentIndex);
            collection.Insert(desiredIndex, item);
        }

        public static bool Contains<T>(this T[] array, T value)
        {
            foreach (T t in array)
            {
                if (t.Equals(value))
                    return true;
            }
            return false;
        }

        public static T[] RemoveFirstElement<T>(this T[] array)
        {
            T[] finalArray = new T[array.Length - 1];
            for (int i = 1; i < array.Length; i++)
            {
                finalArray[i - 1] = array[i];
            }
            return finalArray;
        }

        /// <summary>
        /// Returns a 'looped' index using modulo
        /// Example: index of 9 and array count of 8 will return 1
        /// Example: index of -1 and array count of 8 will return 7
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newIndex"></param>
        /// <returns></returns>
        public static int GetLoopingIndex(this IList collection, int index)
        {
            return (int)MathExtensions.Modulo(index, collection.Count);
        }
    }
}