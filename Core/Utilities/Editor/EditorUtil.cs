using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Core
{
    /// <summary>
    /// Utility class for convenient Editor functions
    /// </summary>
    public static class EditorUtil 
    {
        /// <summary>
        /// Open a script via code, the type name has to correspond with the class name for it to work.
        /// </summary>
        /// <param name="type"></param>
        public static void OpenScript(Type type)
        {
            string fileName = type.Name.ToString() + ".cs";
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (assetPath.EndsWith(fileName))
                {
                    var script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
                    if (script != null)
                    {
                        bool success = AssetDatabase.OpenAsset(script);
                        if (!success)
                            Debug.LogError("Unable to open " + fileName);
                        return;
                    }
                }
            }
            Debug.LogError("Unable to find " + fileName);
        }

        public static void OpenScript(Object obj)
        {
            OpenScript(obj.GetType());
        }
    }
}