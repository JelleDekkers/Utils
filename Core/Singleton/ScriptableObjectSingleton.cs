using System.Linq;
using UnityEngine;

namespace Utils.Core
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    T[] resources = Resources.LoadAll<T>("");

                    if (resources.Length > 0)
					{
                        instance = resources[0];
                    }
                    else
                        Debug.LogError("Resource not found for type: " + typeof(T).Name);
                }
                return instance;
            }
        }
    }
}