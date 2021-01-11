using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.Core.EditorHierarchy
{
    [Serializable]
    public struct InstanceInfo
    {
        /// <summary>
        /// Contains the indexes for each icon to draw, eg. draw(iconTextures[iconIndexes[0]]);
        /// </summary>
        public List<int> iconIndexes;
        public bool isEditorOnly;
        public bool isGameObjectActive;

        public bool isLastElement;
        public bool hasChilds;
        public bool topParentHasChild;

        public int nestingGroup;
        public int nestingLevel;
        public HierarchySeperator seperatorGroup;
        public bool isGroupManager;
    }

    [InitializeOnLoad]
    public class HierarchyDrawer
    {
        static HierarchyDrawer()
        {
            Initialize();
        }

        public static HierarchySettings Settings => HierarchySettings.Instance;

        private static int firstInstanceID = 0;
        private static List<int> iconsPositions = new List<int>();
        private static Dictionary<int, InstanceInfo> sceneGameObjects = new Dictionary<int, InstanceInfo>();

        private static bool drawAlternatingBG;
        private static int iconsCount;
        private static InstanceInfo currentItem;
        private const int iconSize = 15;
        private const float seperatorGroupDisabledTransparency = 0.3f;

        /// <summary>
        /// Initializes the script at the beginning. 
        /// </summary>
        public static void Initialize()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DrawObjectLabel;
            EditorApplication.hierarchyChanged -= RetrieveDataFromScene;

            if (Settings.enabled)
            {
                EditorApplication.hierarchyWindowItemOnGUI += DrawObjectLabel;
                EditorApplication.hierarchyChanged += RetrieveDataFromScene;
                RetrieveDataFromScene();
            }

            Refresh();
        }

        public static void Refresh()
        {
            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        /// Updates the list of objects to draw, icons etc.
        /// </summary>
        static void RetrieveDataFromScene()
        {
            if (!Settings.updateInPlayMode && Application.isPlaying)
                return;

            sceneGameObjects.Clear();
            iconsPositions.Clear();

            GameObject[] sceneRoots;
            Scene tempScene;
            firstInstanceID = -1;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                tempScene = SceneManager.GetSceneAt(i);
                if (tempScene.isLoaded)
                {
                    sceneRoots = tempScene.GetRootGameObjects();
                    //Analyzes all scene's gameObjects
                    for (int j = 0; j < sceneRoots.Length; j++)
                    {
                        HierarchySeperator seperator = sceneRoots[j].GetComponent<HierarchySeperator>();
                        UpdateGameObjectsData(sceneRoots[j], 0, sceneRoots[j].transform.childCount > 0, j, j == sceneRoots.Length - 1, seperator);
                    }

                    if (firstInstanceID == -1 && sceneRoots.Length > 0)
                        firstInstanceID = sceneRoots[0].GetInstanceID();
                }
            }
        }

        private static void UpdateGameObjectsData(GameObject go, int nestingLevel, bool topParentHasChild, int nestingGroup, bool isLastChild, HierarchySeperator parentSeperator)
        {
            int instanceID = go.GetInstanceID();
            HierarchySeperator seperator = go.GetComponent<HierarchySeperator>();

            if (!sceneGameObjects.ContainsKey(instanceID)) //processes the gameobject only if it wasn't processed already
            {
                InstanceInfo newInfo = new InstanceInfo();
                newInfo.iconIndexes = new List<int>();
                newInfo.isLastElement = isLastChild && go.transform.childCount == 0;
                newInfo.nestingLevel = nestingLevel;
                newInfo.nestingGroup = nestingGroup;
                newInfo.hasChilds = go.transform.childCount > 0;
                newInfo.isGameObjectActive = go.activeInHierarchy;
                newInfo.topParentHasChild = topParentHasChild;

                newInfo.isGroupManager = seperator != null;
                if (seperator == null)
                    seperator = parentSeperator;
                newInfo.seperatorGroup = seperator;

                newInfo.isEditorOnly = string.Compare(go.tag, "EditorOnly", StringComparison.Ordinal) == 0;
                sceneGameObjects.Add(instanceID, newInfo);
            }

            int childCount = go.transform.childCount;
            for (int j = 0; j < childCount; j++)
            {
                UpdateGameObjectsData(go.transform.GetChild(j).gameObject, nestingLevel + 1, topParentHasChild, nestingGroup, j == childCount - 1, seperator);
            }
        }

        private static void DrawObjectLabel(int instanceID, Rect rect)
        {
            if (!Settings.enabled || !sceneGameObjects.ContainsKey(instanceID))
                return;

            currentItem = sceneGameObjects[instanceID];

            if (Settings.drawAlternatingBackgrounds)
                DrawAlternatingBackgrounds(instanceID, rect);

            if(Settings.drawSeperatorGroupChildBackground && currentItem.seperatorGroup)
                DrawSeperatorGroupOverlay(rect, currentItem.seperatorGroup.childBackgroundColor, currentItem.nestingLevel);
            
            if (Settings.drawChildBranches)
                DrawTreeBranches(rect);

            if (currentItem.isGroupManager)
                DrawSeperatorGroupHeader(currentItem.seperatorGroup, rect);

            if (currentItem.isEditorOnly && Settings.colorizeEditorOnly)
                ColorizeLabel(rect, Settings.editorOnlyColor);

            if (Settings.drawIcons)
                DrawObjectIcons(instanceID, rect);
        }

        private static void DrawDividerLine(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }

        private static void ColorizeLabel(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }

        private static void DrawSeperatorGroupHeader(HierarchySeperator seperator, Rect rect)
        {
            if (seperator == null)
                return;

            GameObject go = seperator.gameObject;
            Color fontColor = Color.black;
            fontColor.a = (go.activeInHierarchy) ? 1 : seperatorGroupDisabledTransparency;
            Color backgroundColor = seperator.titleBackgroundColor;
            backgroundColor.a = (go.activeSelf) ? 1 : seperatorGroupDisabledTransparency;
            EditorGUI.DrawRect(rect, backgroundColor);

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState() { textColor = fontColor }
            };

            EditorGUI.LabelField(rect, go.name.ToUpper(), style);

            //rect = new Rect(32, rect.y - Settings.dividerLineThickness / 2f, rect.width + (rect.x - 32), Settings.dividerLineThickness);
            //DrawDividerLine(rect, Color.black * 0.3f);
        }

        private static void DrawTreeBranches(Rect rect)
        {
            if (currentItem.nestingLevel >= 1)
            {
                if (rect.x >= 60) //prevents drawing when the hierarchy search mode is enabled 
                {
                    if (currentItem.nestingLevel == 0 && !currentItem.hasChilds)
                    {
                        HierarchyBranchDrawerHelper.DrawHalfVerticalLine(rect, true, 0, Settings.dividerColor);
                        HierarchyBranchDrawerHelper.DrawHalfVerticalLine(rect, false, 0, Settings.dividerColor);
                    }
                    else
                    {
                        //Draws a vertical line for each previous nesting level
                        for (int i = 1; i <= currentItem.nestingLevel; i++)
                        {
                            HierarchyBranchDrawerHelper.DrawVerticalLine(rect, i);
                        }

                        HierarchyBranchDrawerHelper.DrawHorizontalLine(rect, currentItem.nestingLevel, currentItem.hasChilds);
                    }
                }
            }
        }

        private static void DrawAlternatingBackgrounds(int instanceID, Rect rect)
        {
            if (instanceID == firstInstanceID)
                drawAlternatingBG = currentItem.nestingGroup % 2 == 0;

            if (drawAlternatingBG)
            {
                //rect = new Rect(60, rect.y, rect.width + (rect.x - 60), rect.height);
                ColorizeLabel(rect, Settings.alternateBackgroundColor);
                drawAlternatingBG = false;
            }
            else
            {
                drawAlternatingBG = true;
            }
        }

        private static void DrawObjectIcons(int instanceID, Rect selectionRect)
        {
            iconsCount = 0;

            //Draws the gameobject icon, if present
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            GUIContent content = EditorGUIUtility.ObjectContent(obj, null);
            Rect rect = new Rect(selectionRect.xMax - iconSize * (iconsCount + 1) - 2, selectionRect.yMin, iconSize, iconSize);

            if (content.image != null && !string.IsNullOrEmpty(content.image.name))
            {
                if (content.image.name != "GameObject Icon" && content.image.name != "Prefab Icon")
                {
                    DrawIcon(content.image, rect);
                }
            }
        }

        private static void DrawIcon(Texture icon, Rect rect)
        {
            GUI.DrawTexture(new Rect(rect.xMax - iconSize * (iconsCount + 1) , rect.yMin, iconSize, iconSize), icon);
            iconsCount++;
        }

        public static void DrawSeperatorGroupOverlay(Rect originalRect, Color color, int nestLevel)
        {
            originalRect = new Rect(32, originalRect.y, originalRect.width + (originalRect.x - 32), originalRect.height);
            EditorGUI.DrawRect(originalRect, color);
        }
    }
}