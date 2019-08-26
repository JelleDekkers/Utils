﻿using System.Collections;

namespace Utils.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] arr)
        {
            return arr[UnityEngine.Random.Range(0, arr.Length)];
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

    }
}