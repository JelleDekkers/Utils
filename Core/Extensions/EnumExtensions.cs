using System;

namespace Utils.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int Count<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static T Next<T>(this T src) where T : Enum
        {
            T[] enumArray = (T[])Enum.GetValues(src.GetType());
            int index = enumArray.GetLoopingIndex(Array.IndexOf(enumArray, src) + 1);
            return enumArray[index];
        }

        public static T Previous<T>(this T src) where T : Enum
        {
            T[] enumArray = (T[])Enum.GetValues(src.GetType());
            int index = enumArray.GetLoopingIndex(Array.IndexOf(enumArray, src) - 1);
            return enumArray[index];
        }

        public static T Add<T>(this T condition, params T[] add) where T : Enum
        {
            int result = Convert.ToInt32(condition);
            foreach (T addCondition in add)
            {
                result |= Convert.ToInt32(addCondition);
            }
            return (T)Enum.ToObject(typeof(T), result);
        }

        public static T Remove<T>(this T condition, params T[] remove) where T : Enum
        {
            int result = Convert.ToInt32(condition);
            foreach (T removeCondition in remove)
            {
                result &= ~Convert.ToInt32(removeCondition);
            }
            return (T)Enum.ToObject(typeof(T), result);
        }

        public static bool HasFlag<T>(this T condition, params T[] flags) where T : Enum
        {
            foreach (T flag in flags)
            {
                if (condition.HasFlag(flag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}