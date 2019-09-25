using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// Static class for creating a <see cref="ScriptableObject"/> that can be filtered using <see cref="TypeFilterWindow"/>
    /// </summary>
    public class ScriptableObjectCreator
    {
        protected static TypeFilterWindow window;
        protected static ScriptableObjectCreator instance;

        [MenuItem("Assets/Create/New ScriptableObject", priority = 0)]
        private static void Open()
        {
            instance = new ScriptableObjectCreator();
            instance.OpenFilterWindow<ScriptableObject>();
        }

        public virtual void OpenFilterWindow<T>() where T : ScriptableObject
        {
            window = EditorWindow.GetWindow<TypeFilterWindow>(true, "ScriptableObjectCreator");
            window.RetrieveTypes<T>(OnTypeSelectedEvent);
        }

        private static void OnTypeSelectedEvent(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            string path = ScriptableObjectUtility.GetSelectionAssetPath(type.Name);

            if (path == null)
                throw new ArgumentNullException("path");

            ScriptableObject asset = ScriptableObjectUtility.CreateAssetAtPath(type, path, "New " + type.Name);
            EditorUtility.FocusProjectWindow();
            EditorUtility.SetDirty(asset);
            Selection.activeObject = asset;

            window.Close();
            instance = null;
        }
    }
}