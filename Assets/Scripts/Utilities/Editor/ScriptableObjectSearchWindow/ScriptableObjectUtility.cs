using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;

using Object = UnityEngine.Object;

/// <summary>
/// Static utility class for creating and destroying assets
/// </summary>
public static class ScriptableObjectUtility
{
    /// <summary>
    /// Create asset in folder, will create a new folder if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">Asset type</typeparam>
    /// <param name="path">Full path to folder</param>
    /// <returns></returns>
    public static ScriptableObject CreateAssetAtPath(Type type, string path, string assetName)
    {
        ScriptableObject asset = ScriptableObject.CreateInstance(type);

        string extensionType = ".Asset";
        string fullPath = path + "/" + assetName + extensionType;
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        AssetDatabase.CreateAsset(asset, fullPath);
        AssetDatabase.ImportAsset(path); 
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return asset;
    }

    /// <summary>
    /// Removes alls scriptableObjects in path of type T
    /// </summary>
    /// <typeparam name="T">Type to remove</typeparam>
    /// <param name="path">path from Resources</param>
    public static void RemoveAssetsFromFolder<T>(string path) where T : ScriptableObject
    {
        T[] assets = Resources.LoadAll(path, typeof(T)).Cast<T>().ToArray();

        foreach (T asset in assets)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
        }
    }

    /// <summary>
    /// Returns selection folder path
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetSelectionAssetPath(string fileName)
    {
        Object selectedObject = Selection.objects.FirstOrDefault();
        string path = "Assets";
        if (selectedObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(selectedObject);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (Directory.Exists(selectedPath))
                {
                    path = selectedPath;
                }
                else
                {
                    path = Path.GetDirectoryName(selectedPath);
                }
            }
        }

        return path;
    }
}
