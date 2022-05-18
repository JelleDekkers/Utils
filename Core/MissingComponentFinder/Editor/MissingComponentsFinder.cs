using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// Editor window for finding all gameobjects with missing components
    /// </summary>
    public class MissingComponentsFinder : EditorWindow
    {
        private readonly Color32 selectionBackgroundColor = new Color32(6, 13, 38, 255);
        private const int TEXT_FONT_SIZE = 12;
        private const int SUBTEXT_FONT_SIZE = 9;

        private List<MissingComponentInfo> missingComponentInfos = new List<MissingComponentInfo>();
        private int totalMissingComponentCount;

        private Vector2 scrollPosition;
        private MissingComponentInfo selectedInfo;

        [MenuItem("Utils/Find Missing Components")]
        private static void Open()
        {
            MissingComponentsFinder instance = GetWindow<MissingComponentsFinder>(true, "Missing Components Finder");
            instance.FindMissingComponents();
        }

        private void OnFocus()
        {
            FindMissingComponents();
        }

        private void FindMissingComponents()
        {
            missingComponentInfos.Clear();
            totalMissingComponentCount = 0;

            Transform[] transforms = FindObjectsOfType<Transform>(true);
            missingComponentInfos = new List<MissingComponentInfo>();

            foreach (Transform item in transforms)
            {
                CheckMissingComponents(item.gameObject);
            }
        }

        protected virtual void OnGUI()
        {
            DrawHeader();
            DrawContent();
        }

        protected virtual void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(EditorGUIUtility.IconContent("d_Refresh"), GUILayout.MaxWidth(30)))
            {
                FindMissingComponents();
            }

            if (missingComponentInfos.Count == 0)
            {
                GUILayout.Label("No missing components found");
            }
            else
            {
                GUILayout.Label(EditorGUIUtility.IconContent("d_console.warnicon"), GUILayout.MaxWidth(20), GUILayout.MaxHeight(20));
                GUILayout.Label("Found " + totalMissingComponentCount + " missing component(s)");
            }

            GUI.enabled = missingComponentInfos.Count > 0;
            if(GUILayout.Button("Select All"))
            {
                Selection.objects = missingComponentInfos.Select(info => info.gameObject).ToArray();
                selectedInfo = null;
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            DrawLine(1);
        }

        private void DrawLine(int thickness)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            rect.height = thickness;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        protected void DrawContent()
        {
            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < missingComponentInfos.Count; i++)
            {
                DrawElement(missingComponentInfos[i]);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        protected Rect DrawElement(MissingComponentInfo info)
        {
            Color backgroundColor = GUI.backgroundColor;

            if (info == selectedInfo)
            {
                GUI.backgroundColor = selectionBackgroundColor;
            }

            Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            GUIStyle textStyle = EditorStyles.label;
            textStyle.fontSize = TEXT_FONT_SIZE;
            string text = info.gameObject != null ? info.gameObject.name : "Object was destroyed";
            GUILayout.Label(text, textStyle);

            GUILayout.FlexibleSpace();

            if (info.missingComponentCount > 1)
            {
                text = info.missingComponentCount + " components";
                textStyle.fontSize = SUBTEXT_FONT_SIZE;
                GUILayout.Label(text, textStyle);
            }

            GUI.enabled = info.gameObject != null;
            if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
                SelectGameObject(info);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = backgroundColor;

            return rect;
        }

        private void SelectGameObject(MissingComponentInfo info)
        {
            Selection.activeObject = info.gameObject;
            selectedInfo = info;
            EditorGUIUtility.PingObject(Selection.activeObject);
        }

        private void CheckMissingComponents(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();
            MissingComponentInfo info = null;
            foreach (Component component in components)
            {
                if (component == null)
                {
                    if (info == null)
                    {
                        info = new MissingComponentInfo(gameObject);
                        missingComponentInfos.Add(info);
                    }
                    info.missingComponentCount++;
                    totalMissingComponentCount++;
                }
            }
        }

        [System.Serializable]
        public class MissingComponentInfo
        {
            public GameObject gameObject;
            public int missingComponentCount;

            public MissingComponentInfo(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }
        }
    }
}