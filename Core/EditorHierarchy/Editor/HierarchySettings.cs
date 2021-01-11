﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    public class HierarchySettings : ScriptableObject
    {
        public static HierarchySettings Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateNewSettingsFile();
                return instance;
            }
        }
        private static HierarchySettings instance;

        public const string AssetPath = "Assets/Utils/Core/EditorHierarchy/Resources/Settings.asset";

        public bool enabled = true;
        public bool updateInPlayMode = true;
        public bool drawIcons = true;

        public bool drawAlternatingBackgrounds = true;
        public Color alternateBackgroundColor = new Color(0.4f, 0.4f, 0.4f, .08f);

        public bool colorizeEditorOnly = true;
        public Color editorOnlyColor = new Color(1, 0.25f, 0, 0.15f);

        public bool drawChildBranches = true;
        public List<Color> childBranchColors = null;

        public bool drawSeperatorGroupChildBackground = true;
        public Color seperatorGroupChildBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        public Color seperatorTitleBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);

        public float dividerLineThickness = 1;
        public Color dividerColor = Color.gray;

        private void OnValidate()
        {
            HierarchyDrawer.Refresh();
        }

        private static HierarchySettings CreateNewSettingsFile()
        {
            HierarchySettings newInstance = AssetDatabase.LoadAssetAtPath<HierarchySettings>(AssetPath);
            if (newInstance == null)
            {
                newInstance = CreateInstance<HierarchySettings>();
                AssetDatabase.CreateAsset(newInstance, AssetPath);
                AssetDatabase.SaveAssets();
            }
            return newInstance;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(CreateNewSettingsFile());
        }
    }   
}