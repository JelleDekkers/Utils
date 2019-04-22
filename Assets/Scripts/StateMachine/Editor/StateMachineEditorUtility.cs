using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Utility class for state machine related assets
    /// </summary>
    public static class StateMachineEditorUtility
    {
        public static T CreateObjectInstance<T>(string assetFilePath) where T : ScriptableObject
        {
            return CreateObjectInstance(typeof(T), assetFilePath) as T;
        }

        public static ScriptableObject CreateObjectInstance(Type type, string assetFilePath)
        {
            ScriptableObject instance = ScriptableObject.CreateInstance(type);
            instance.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(instance, assetFilePath);
            //AssetDatabase.ImportAsset(assetFilePath);
            return instance;
        }
    }
}