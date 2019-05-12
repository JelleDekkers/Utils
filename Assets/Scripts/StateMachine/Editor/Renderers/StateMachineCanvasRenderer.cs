using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering the canvas window for a <see cref="StateMachineData"/>
    /// </summary>
    public class StateMachineCanvasRenderer
    {
        public const float CANVAS_WIDTH = 1000;
        public const float CANVAS_HEIGHT = 1000;

        private const float GRID_LINE_INTERVAL = 50f;
        private const float WINDOW_HEIGHT = 400;

        public StateMachineEditorManager Manager { get; private set; }
        public Rect CanvasWindow { get; private set; }
        public Vector2 CanvasDrag { get; private set; }

        private readonly Color backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        private readonly Color gridPrimaryColor = new Color(0, 0, 0, 0.18f);
        private readonly Color gridSecondaryColor = new Color(0, 0, 0, 0.1f);
        private readonly float gridPrimarySpacing = 20f;
        private readonly float gridSecondarySpacing = 100f;

        private float zoomScale = 100;

        public StateMachineCanvasRenderer(StateMachineEditorManager manager)
        {
            Manager = manager;
        }

        public void OnInspectorGUI(Event e)
        {
            DrawTopTabs();
            DrawCanvasWindow(e);
            DrawBottomTabs();
        }

        private void DrawCanvasWindow(Event e)
        {
            // Hack to prevent strange issue where EditorGUILayout.BeginVertical returns zero every other frame causing values to be wrong
            Rect temp = EditorGUILayout.BeginVertical(GUILayout.Height(WINDOW_HEIGHT));
            Rect scrollView = CanvasWindow;
            if (temp != Rect.zero)
            {
                scrollView = temp;
            }

            Vector2 windowPos = EditorGUILayout.BeginScrollView(CanvasDrag, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Height(WINDOW_HEIGHT));
            CanvasWindow = new Rect(windowPos, scrollView.size);

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            GUILayout.Box(GUIContent.none, GUILayout.Width(CANVAS_WIDTH), GUILayout.Height(CANVAS_HEIGHT));
            GUI.backgroundColor = oldColor;

            DrawGrid(gridPrimarySpacing, gridPrimaryColor);
            DrawGrid(gridSecondarySpacing, gridSecondaryColor);
            DrawStates();
            ProcessStateEvents(e);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:

                    if (e.button == 1 && CanvasWindow.Contains(e.mousePosition)) 
                    {
                        ShowContextMenu(e);
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (CanvasWindow.Contains(e.mousePosition))
                        {
                            Drag(e.delta);
                            e.Use();
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (CanvasWindow.Contains(e.mousePosition))
                        {
                            e.Use();
                        }
                    }
                    break;
            }


            // TODO: werkend krijgen met scrollwheel
            //if (currentEvent.isMouse)
            //{
            //    //Debug.Log(currentEvent.delta);
            //    zoomScale += currentEvent.delta.y;
            //}
        }

        public bool Contains(Vector2 pos)
        {
            return CanvasWindow.Contains(pos);
        }

        public Vector2 GetWindowCentre()
        {
            return new Vector2(CanvasWindow.position.x + CanvasWindow.width / 2 - StateRenderer.WIDTH / 2, CanvasWindow.position.y + CanvasWindow.height / 2);
        }

        private void ProcessStateEvents(Event e)
        {
            for (int i = Manager.StateRenderers.Count - 1; i >= 0; i--)
            {
                Manager.StateRenderers[i].ProcessEvents(e);
            }
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New State"), false, () => Manager.CreateNewState(e.mousePosition));
            menu.ShowAsContext();

            e.Use();
        }

        private void Drag(Vector2 delta)
        {
            Vector2 drag = CanvasDrag;
            drag -= delta;
            drag.x = Mathf.Clamp(drag.x, 0, CANVAS_WIDTH);
            drag.y = Mathf.Clamp(drag.y, 0, CANVAS_HEIGHT);

            CanvasDrag = drag;
            GUI.changed = true;
        }

        private void DrawGrid(float gridSpacing, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(CANVAS_WIDTH / gridSpacing);
            int heightDivs = Mathf.CeilToInt(CANVAS_HEIGHT / gridSpacing);

            Handles.BeginGUI();
            Handles.color = gridColor;

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0), new Vector3(gridSpacing * i, CANVAS_HEIGHT, 0f));
            }

            for (int i = 0; i < heightDivs; i++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0), new Vector3(CANVAS_WIDTH, gridSpacing * i, 0f));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawStates()
        {
            for (int i = 0; i < Manager.StateRenderers.Count; i++)
            {
                Manager.StateRenderers[i].Draw();
            }
        }

        private void DrawTopTabs()
        {
            float maxTabWidth = 150;
            float groupSpace = 10;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("New State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.CreateNewState();
            }

            GUI.enabled = Manager.Selection != null && Manager.Selection is StateRenderer;
            if (GUILayout.Button("Delete State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.RemoveState(Manager.Selection as StateRenderer);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.ResetState((Manager.Selection as StateRenderer).State);
            }
            GUI.enabled = true;

            GUILayout.Space(groupSpace);

            if (GUILayout.Button("Clear Machine", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.ClearStateMachine();

            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomTabs()
        {
            float groupSpace = 10;
            float maxTabWidth = 150;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.MaxWidth(25)))
            {
                //SetCanvasScale(Scale + SCALE_INTERVAL);
            }

            if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.MaxWidth(25)))
            {
                //SetCanvasScale(Scale - SCALE_INTERVAL);
            }

            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.PrefixLabel(zoomScale.ToString() + "%", EditorStyles.toolbarTextField);

            if (GUILayout.Button("Reset View", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                ResetView();
            }

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleRight;
            string stateMachineName = AssetDatabase.GetAssetPath(Manager.StateMachineData);
            EditorGUILayout.LabelField(stateMachineName.Replace(".asset", ""), style);

            GUILayout.Space(groupSpace);
            Manager.debug = GUILayout.Toggle(Manager.debug, "Debug", EditorStyles.toolbarButton, GUILayout.MaxWidth(50));

            EditorGUILayout.EndHorizontal();
        }

        private void SetCanvasScale(float scale)
        {
            // set zoom scale
        }

        private void ResetView()
        {
            CanvasDrag = Vector2.zero;
            zoomScale = 100;
        }
    }
}