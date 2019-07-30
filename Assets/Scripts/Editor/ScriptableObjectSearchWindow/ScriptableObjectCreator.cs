using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Static class for creating a <see cref="ScriptableObject"/> that can be filtered using <see cref="TypeFilterWindow"/>
/// </summary>
public static class ScriptableObjectCreator
{
    private static TypeFilterWindow window;

    [MenuItem("Assets/Create/ScriptableObject", priority = 0)]
    private static void Open()
    {
        window = EditorWindow.GetWindow<TypeFilterWindow>(true, "ScriptableObject");
        window.RetrieveTypes<ScriptableObject>(OnTypeSelectedEvent);
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
    }
}
