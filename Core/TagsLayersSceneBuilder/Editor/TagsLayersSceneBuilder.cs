﻿using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor class for generating classes which contain constant variables for Tags, Layers and Scenes
/// </summary>
public class TagsLayersScenesBuilder : EditorWindow
{
    private const string FOLDER_LOCATION = "Scripts/AutoGenerated/";
    private const string TAGS_FILE_NAME = "Tags";
    private const string LAYERS_FILE_NAME = "Layers";
    private const string SCENES_FILE_NAME = "Scenes";
    private const string SCRIPT_EXTENSION = ".cs";

    [MenuItem("Utils/Rebuild Tags, Layers and Scenes Classes")]
    static void RebuildTagsAndLayersClasses()
    {
        string folderPath = Application.dataPath + "/" + FOLDER_LOCATION;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        CreateClass(folderPath, TAGS_FILE_NAME, SCRIPT_EXTENSION, GetClassContent(TAGS_FILE_NAME, UnityEditorInternal.InternalEditorUtility.tags));
        CreateClass(folderPath, LAYERS_FILE_NAME, SCRIPT_EXTENSION, GetLayerClassContent(LAYERS_FILE_NAME, UnityEditorInternal.InternalEditorUtility.layers));
        CreateClass(folderPath, SCENES_FILE_NAME, SCRIPT_EXTENSION, GetClassContent(SCENES_FILE_NAME, EditorBuildSettingsScenesToNameStrings(EditorBuildSettings.scenes)));
        AssetDatabase.Refresh();
        Debug.Log("Rebuild Complete");
    }

    private static void CreateClass(string folderPath, string fileName, string extension, string classContent)
    {
        File.WriteAllText(folderPath + fileName + extension, classContent);
        AssetDatabase.ImportAsset("Assets/" + FOLDER_LOCATION + fileName + extension, ImportAssetOptions.ForceUpdate);
    }

    private static string[] EditorBuildSettingsScenesToNameStrings(EditorBuildSettingsScene[] scenes)
    {
        string[] sceneNames = new string[scenes.Length];
        for (int n = 0; n < sceneNames.Length; n++)
        {
            sceneNames[n] = Path.GetFileNameWithoutExtension(scenes[n].path);
        }
        return sceneNames;
    }

    private static string GetClassContent(string className, string[] labelsArray)
    {
        string output = "";
        output += "//This class is auto-generated do not modify (TagsLayersScenesBuilder.cs)\n";
        output += "public class " + className + "\n";
        output += "{\n";
        foreach (string label in labelsArray)
        {
            output += "\t" + BuildConstVariable(label) + "\n";
        }
        output += "}";
        return output;
    }

    private static string GetLayerClassContent(string className, string[] labelsArray)
    {
        string output = "";
        output += "//This class is auto-generated do not modify (TagsLayersScenesBuilder.cs)\n";
        output += "public class " + className + "\n";
        output += "{\n";
        foreach (string label in labelsArray)
        {
            output += "\t" + BuildConstVariable(label) + "\n";
        }
        output += "\n";

        foreach (string label in labelsArray)
        {
            output += "\t" + "public const int " + ToUpperCaseWithUnderscores(label) + "_INT" + " = " + LayerMask.NameToLayer(label) + ";\n";
        }

        output += "}";
        return output;
    }

    private static string BuildConstVariable(string varName)
    {
        return "public const string " + ToUpperCaseWithUnderscores(varName) + " = " + '"' + varName + '"' + ";";
    }

    private static string ToUpperCaseWithUnderscores(string input)
    {
        string output = "" + input[0];

        for (int n = 1; n < input.Length; n++)
        {
            if ((char.IsUpper(input[n]) || input[n] == ' ') && !char.IsUpper(input[n - 1]) && input[n - 1] != '_' && input[n - 1] != ' ')
            {
                output += "_";
            }

            if (input[n] != ' ' && input[n] != '_')
            {
                output += input[n];
            }
        }

        output = output.ToUpper();
        return output;
    }
}