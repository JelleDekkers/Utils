using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class InterfaceExtensions
    {
        public static T GetInterface<T>(this GameObject obj) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an interface!");
                return null;
            }
            return obj.GetComponent<T>() as T;
        }

        public static T GetInterface<T>(this GameObject obj, System.Type type) where T : class
        {
            if (!type.IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an interface!");
                return null;
            }
            return obj.GetComponent(type) as T;
        }

        public static bool TryGetInterface<T>(this GameObject obj, out T @interface) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new System.Exception(typeof(T).ToString() + ": is not an interface!");
            }

            @interface = obj.GetComponent<T>() as T;
            return @interface != null;
        }

        public static T[] GetInterfaces<T>(this GameObject obj) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an interface!");
                return null; ;
            }

            return obj.GetComponents<T>() as T[];
        }
    }
}
