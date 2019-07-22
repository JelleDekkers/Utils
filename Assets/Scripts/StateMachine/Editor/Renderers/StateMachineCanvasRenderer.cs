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
        public const float SCROLLVIEW_WIDTH = 1000;
        public const float SCROLLVIEW_HEIGHT = 1000;

        private const float WINDOW_HEIGHT = 400;
        private const float DRAG_THRESHOLD = 5;
        private const float ZOOM_SCALE_INTERVAL = 0.1f;
        private const float ZOOM_SCALE_MAX = 1f;
        private const float ZOOM_SCALE_MIN = 0.5f;

        public StateMachineEditorManager Manager { get; private set; }
        public Rect Window { get; private set; }
        public Vector2 ScrollViewDrag { get; private set; }

        private readonly Color backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        private readonly Color gridPrimaryColor = new Color(0, 0, 0, 0.18f);
        private readonly Color gridSecondaryColor = new Color(0, 0, 0, 0.2f);
        private readonly float gridPrimarySpacing = 20f;
        private readonly float gridSecondarySpacing = 100f;

        public Rect scrollView; // TODO: private
        private float zoomScale = 1f;
        private Vector2 dragStartPos;
        private bool canDrag;

        public StateMachineCanvasRenderer(StateMachineEditorManager manager)
        {
            Manager = manager;
            scrollView = new Rect(Vector2.zero, new Vector2(SCROLLVIEW_WIDTH, SCROLLVIEW_HEIGHT));
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
            Rect r = EditorGUILayout.BeginVertical(GUILayout.Height(WINDOW_HEIGHT));

            //Matrix4x4 prevGuiMatrix = GUI.matrix;
            //Matrix4x4 translation = Matrix4x4.TRS(scrollView.position, Quaternion.identity, Vector3.one);
            //Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1f));
            //GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            Vector2 windowPos = EditorGUILayout.BeginScrollView(ScrollViewDrag, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Height(WINDOW_HEIGHT));
            Window = new Rect(windowPos, new Vector2(r.width, WINDOW_HEIGHT));

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            GUILayout.Box(GUIContent.none, GUILayout.Width(SCROLLVIEW_WIDTH), GUILayout.Height(SCROLLVIEW_HEIGHT));
            GUI.backgroundColor = oldColor;

            //GUIUtility.ScaleAroundPivot(new Vector2(zoomScale, zoomScale), Vector2.zero);

            DrawGrid(gridPrimarySpacing, gridPrimaryColor);
            DrawGrid(gridSecondarySpacing, gridSecondaryColor);

            DrawStates();
            ProcessStateEvents(e);
            ProcessEvents(e);

            //GUI.matrix = prevGuiMatrix;
            //GUI.matrix = Matrix4x4.identity;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1 && Window.Contains(e.mousePosition)) 
                    {
                        ShowContextMenu(e.mousePosition);
                        e.Use();
                        break;
                    }

                    if(e.button == 0)
                    {
                        dragStartPos = e.mousePosition;
                        canDrag = false;
                    }

                    Manager.ContextMenuIsOpen = false;
                    break;

                case EventType.MouseDrag:
                    if (!Manager.ContextMenuIsOpen && e.button == 0)
                    {
                        if (Window.Contains(e.mousePosition))
                        {
                            Drag(e); 
                            e.Use();
                        }
                    }
                    break;

                case EventType.KeyDown:
                    if(e.keyCode == KeyCode.Delete)
                    {
                        Manager.StateMachineData.RemoveState((Manager.Selection as StateRenderer).State);
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
            return Window.Contains(pos);
        }

        public Vector2 GetWindowCentre()
        {
            return new Vector2(Window.position.x + Window.width / 2 - StateRenderer.WIDTH / 2, Window.position.y + Window.height / 2);
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

        private void Drag(Event e)
        {
            float dragDif = (dragStartPos - e.mousePosition).magnitude;
            if (dragDif > DRAG_THRESHOLD)
            {
                canDrag = true;
            }

            if (canDrag)
            {
                Vector2 drag = ScrollViewDrag;
                drag -= e.delta;
                drag.x = Mathf.Clamp(drag.x, 0, scrollView.width - Window.width + 5);
                drag.y = Mathf.Clamp(drag.y, 0, scrollView.height - Window.height + 5);

                ScrollViewDrag = drag;
                GUI.changed = true;
            }
        }

        private void DrawGrid(float gridSpacing, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(SCROLLVIEW_WIDTH / gridSpacing);
            int heightDivs = Mathf.CeilToInt(SCROLLVIEW_HEIGHT / gridSpacing);

            Handles.BeginGUI();
            Handles.color = gridColor;

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0), new Vector3(gridSpacing * i, SCROLLVIEW_HEIGHT, 0f));
            }

            for (int i = 0; i < heightDivs; i++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * i, 0), new Vector3(SCROLLVIEW_WIDTH, gridSpacing * i, 0f));
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
                Manager.StateMachineData.RemoveState((Manager.Selection as StateRenderer).State);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                (Manager.Selection as StateRenderer).State.ClearActions();
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

            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.PrefixLabel((zoomScale * 100).ToString("#") + "%", EditorStyles.toolbarTextField);

            zoomScale = GUILayout.HorizontalSlider(zoomScale, ZOOM_SCALE_MIN, ZOOM_SCALE_MAX, GUILayout.MaxWidth(100));

            if (GUILayout.Button("Reset View", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                ResetView();
            }

            GUIStyle style = new GUIStyle("Toolbar");
            style.alignment = TextAnchor.MiddleRight;
            style.normal.background = null;
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

        private void SetCanvasZoomScale(float newScale)
        {
            zoomScale = Mathf.Clamp(newScale, ZOOM_SCALE_MIN, ZOOM_SCALE_MAX);
        }

        private void ResetView()
        {
            ScrollViewDrag = Vector2.zero;
            zoomScale = 1f;
        }
    }
}