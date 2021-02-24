using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    public static class HierarchySettingsMenu
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/Editor Hierarchy", SettingsScope.User)
            {
                label = "Hierarchy",
                guiHandler = (searchContext) =>
                {
                    SerializedObject serializedSettings = HierarchySettings.GetSerializedSettings();
                    HierarchySettings settings = HierarchySettings.Instance;

                    GUILayout.Label("General", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("enabled"), new GUIContent("Enabled"));
                    GUI.enabled = settings.enabled;
                    //EditorGUILayout.PropertyField(serializedSettings.FindProperty("updateInPlayMode"), new GUIContent("UpdateInPlayMode"));
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("drawIcons"), new GUIContent("Draw Icons"));

                    EditorGUILayout.Space(10);
                    GUILayout.Label("Background", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("drawAlternatingBackgrounds"), new GUIContent("Alternating BG colors"));
                    GUI.enabled = settings.drawAlternatingBackgrounds;
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("alternateBackgroundColor"), new GUIContent("Alternate color"));
                    GUI.enabled = settings.enabled;

                    EditorGUILayout.Space(10);
                    GUILayout.Label("Editor Only Tag", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("colorizeEditorOnly"), new GUIContent("Colorize Editor Only"));
                    GUI.enabled = settings.colorizeEditorOnly;
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("editorOnlyColor"), new GUIContent("Editor Only Tag color"));
                    GUI.enabled = settings.enabled;
                    
                    EditorGUILayout.Space(10);
                    GUILayout.Label("Seperator", EditorStyles.boldLabel);
                    GUILayout.Label("(Applies to new objects only)", EditorStyles.miniLabel);
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("seperatorTitleBackgroundColor"), new GUIContent("Seperator BG Color"));
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("drawSeperatorGroupChildBackground"), new GUIContent("Draw Child Background"));
                    GUI.enabled = settings.drawSeperatorGroupChildBackground;
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("seperatorGroupChildBackgroundColor"), new GUIContent("Children BG color"));
                    GUI.enabled = settings.enabled;

                    EditorGUILayout.Space(10);
                    GUILayout.Label("Child Branches", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("drawChildBranches"), new GUIContent("Draw child branches"));
                    GUI.enabled = settings.drawChildBranches;
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("childBranchColors"), new GUIContent("Colors"));
                    if (settings.childBranchColors != null && settings.childBranchColors.Count == 0)
                        settings.childBranchColors.Add(HierarchySettings.BranchFallbackColor);
                    GUI.enabled = settings.enabled;

                    GUI.enabled = true;
                    serializedSettings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Hierarchy", "Editor" })
            };


            return provider;
        }
    }
}