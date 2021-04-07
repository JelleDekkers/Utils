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
                    instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                return instance;
            }
        }
    }
}