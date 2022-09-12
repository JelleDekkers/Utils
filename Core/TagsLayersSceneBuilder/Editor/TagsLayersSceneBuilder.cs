using System.IO;
using UnityEditor;
using UnityEngine;
using Utils.Core.ScriptBuilder;

/// <summary>
/// Editor class for generating classes which contain constant variables for Tags, Layers and Scenes
/// </summary>
public class TagsLayersScenesBuilder
{
    private const string TAGS_FILE_NAME = "Tags";
    private const string LAYERS_FILE_NAME = "Layers";
    private const string SCENES_FILE_NAME = "Scenes";

    private ScriptBuilder scriptBuilder;

    [MenuItem("Utils/Rebuild Tags, Layers and Scenes Classes")]
    static void RebuildTagsAndLayersClasses()
    {
        TagsLayersScenesBuilder builder = new TagsLayersScenesBuilder();
        builder.BuildClasses();
    }

    private void BuildClasses()
    {
        scriptBuilder = new ScriptBuilder();

        string folderPath = Application.dataPath + "/" + ScriptBuilder.DEFAULT_FOLDER_LOCATION;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        scriptBuilder.CreateScript(folderPath, TAGS_FILE_NAME, scriptBuilder.GetClassContent(TAGS_FILE_NAME, UnityEditorInternal.InternalEditorUtility.tags));
        scriptBuilder.CreateScript(folderPath, LAYERS_FILE_NAME, GetLayerClassContent(LAYERS_FILE_NAME, UnityEditorInternal.InternalEditorUtility.layers));
        scriptBuilder.CreateScript(folderPath, SCENES_FILE_NAME, scriptBuilder.GetClassContent(SCENES_FILE_NAME, EditorBuildSettingsScenesToNameStrings(EditorBuildSettings.scenes)));
        AssetDatabase.Refresh();
        Debug.Log("Rebuild Complete");
    }

    private string GetLayerClassContent(string className, string[] labelsArray)
    {
        string output = "";
        output += "//This class is auto-generated do not modify (TagsLayersScenesBuilder.cs)\n";
        output += "public class " + className + "\n";
        output += "{\n";
        foreach (string label in labelsArray)
        {
            output += "\t" + scriptBuilder.BuildConstVariable(label) + "\n";
        }
        output += "\n";

        foreach (string label in labelsArray)
        {
            output += "\t" + "public const int " + scriptBuilder.ToUpperCaseWithUnderscores(label) + "_INT" + " = " + LayerMask.NameToLayer(label) + ";\n";
        }

        output += "}";
        return output;
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
}