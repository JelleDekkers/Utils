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

        private Rect scrollView = new Rect();
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
            ProcessEvents(e);

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
                        ShowContextMenu(e.mousePosition);
                        e.Use();
                        break;
                    }

                    Manager.ContextMenuIsOpen = false;
                    break;

                case EventType.MouseDrag:
                    if (!Manager.ContextMenuIsOpen && e.button == 0)
                    {
                        if (CanvasWindow.Contains(e.mousePosition))
                        {
                            Drag(e.delta); 
                            e.Use();
                        }
                    }
                    break;

                case EventType.KeyDown:
                    if(e.keyCode == KeyCode.Delete)
                    {
                        Manager.StateMachineData.RemoveState((Manager.Selection as StateRenderer).DataObject);
                        e.Use();
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

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New State"), false, () => Manager.StateMachineData.CreateNewState(mousePosition));

            // if clipboard is of type State
            //menu.AddItem(new GUIContent("Paste State"), false, () => throw new NotImplementedException());

            menu.ShowAsContext();
            Manager.ContextMenuIsOpen = true;
            GUI.changed = true;
        }

        private void Drag(Vector2 delta)
        {
            Vector2 drag = CanvasDrag;
            drag -= delta;
            drag.x = Mathf.Clamp(drag.x, 0, CanvasWindow.width);
            drag.y = Mathf.Clamp(drag.y, 0, CanvasWindow.height);

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
                Manager.StateMachineData.CreateNewState(GetWindowCentre());
            }

            GUI.enabled = Manager.Selection != null && Manager.Selection is StateRenderer;
            if (GUILayout.Button("Delete State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.StateMachineData.RemoveState((Manager.Selection as StateRenderer).DataObject);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                (Manager.Selection as StateRenderer).DataObject.ClearActions();
            }
            GUI.enabled = true;

            GUILayout.Space(groupSpace);

            if (GUILayout.Button("Clear Machine", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Manager.StateMachineData.ClearStateMachine();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomTabs()
        {
            float groupSpace = 10;
            float maxTabWidth = 100;

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
            bool debug = Manager.ShowDebug;
            debug = GUILayout.Toggle(Manager.ShowDebug, "Debug", EditorStyles.toolbarButton, GUILayout.MaxWidth(50));
            if(debug != Manager.ShowDebug)
            {
                Manager.SetDebug(debug);
            }

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