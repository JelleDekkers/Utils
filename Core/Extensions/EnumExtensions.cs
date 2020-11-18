using System;

namespace Utils.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int Count<T>()
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static T Next<T>(this T src) where T : struct
        {
            T[] enumArray = (T[])Enum.GetValues(src.GetType());
            int index = enumArray.GetLoopingIndex(Array.IndexOf(enumArray, src) + 1);
            return enumArray[index];
        }

        public static T Previous<T>(this T src) where T : struct
        {
            T[] enumArray = (T[])Enum.GetValues(src.GetType());
            int index = enumArray.GetLoopingIndex(Array.IndexOf(enumArray, src) - 1);
            return enumArray[index];
        }
    }
}