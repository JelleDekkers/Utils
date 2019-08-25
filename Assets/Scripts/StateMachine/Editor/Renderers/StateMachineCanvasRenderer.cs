using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for rendering the canvas window for a <see cref="StateMachineScriptableObjectData"/>
    /// </summary>
    public class StateMachineCanvasRenderer
    {
        public const float SCROLL_VIEW_WIDTH = 10000;
        public const float SCROLL_VIEW_HEIGHT = 10000;

        private const float MIN_WINDOW_HEIGHT = 400;
        private const float DRAG_THRESHOLD = 5;
        private const float ZOOM_SCALE_MAX = 1.5f;
        private const float ZOOM_SCALE_MIN = 0.5f;

        public StateMachineLayerRenderer EditorUI { get; private set; }
        public Vector2 ScrollViewDrag { get; private set; }
        public Rect windowRect = new Rect(0, 0, 0, MIN_WINDOW_HEIGHT);

        private readonly Color backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        private readonly Color gridPrimaryColor = new Color(0, 0, 0, 0.18f);
        private readonly Color gridSecondaryColor = new Color(0, 0, 0, 0.2f);
        private readonly float gridPrimarySpacing = 20f;
        private readonly float gridSecondarySpacing = 100f;

        private Rect scrollView; 
        private float zoomScale = 1f;
        private Vector2 dragStartPos;
        private bool dragThresholdReached;
        private HorizontalResizeHandle resizeHandle;

        public StateMachineCanvasRenderer(StateMachineLayerRenderer editorUI)
        {
            EditorUI = editorUI;

            scrollView = new Rect(Vector2.zero, new Vector2(SCROLL_VIEW_WIDTH, SCROLL_VIEW_HEIGHT));
            resizeHandle = new HorizontalResizeHandle(MIN_WINDOW_HEIGHT, float.MaxValue);
        }

        public void OnInspectorGUI(Event e)
        {
            DrawTopToolbar();
            DrawCanvasWindow(e);
            DrawBottomToolbar();

            EditorGUILayout.Space();
            resizeHandle.ProcessEvents(e, ref windowRect);
            resizeHandle.Draw();
        }

        private void DrawCanvasWindow(Event e)
        {
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            Rect canvasArea = EditorGUILayout.BeginVertical(NodeGUIStyles.CanvasWindowStyle, GUILayout.Height(windowRect.height));
            GUI.backgroundColor = oldColor;

            Vector2 scrollViewPos = EditorGUILayout.BeginScrollView(ScrollViewDrag, false, false, GUIStyle.none, GUIStyle.none, new GUIStyle(), GUILayout.Height(canvasArea.height));

            // For some reason rect is 0 every other frame, this prevents that
            if (canvasArea.size != Vector2.zero)
            {
                windowRect = new Rect(scrollViewPos, new Vector2(canvasArea.width, windowRect.height));
            }

            oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            // This box functions as the internal view area of the scrollview, this is required for the scroll area to work, since nodes are drawn without GUILayout
            GUILayout.Box(GUIContent.none, GUILayout.Width(SCROLL_VIEW_WIDTH), GUILayout.Height(SCROLL_VIEW_HEIGHT));
            GUI.backgroundColor = oldColor;

            DrawGrid(gridPrimarySpacing, gridPrimaryColor);
            DrawGrid(gridSecondarySpacing, gridSecondaryColor);

            DrawNodes();
            ProcessNodeRendererEvents(e);
            ProcessEvents(e);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1 && windowRect.Contains(e.mousePosition)) 
                    {
                        ShowContextMenu(e.mousePosition);
                        e.Use();
                        break;
                    }

                    if (e.button == 0)
                    {
                        if (windowRect.Contains(e.mousePosition))
                        {
                            dragStartPos = e.mousePosition;
                            dragThresholdReached = false;
                        }
                    }

                    EditorUI.ContextMenuIsOpen = false;
                    break;

                case EventType.MouseDrag:

                    if (!EditorUI.ContextMenuIsOpen && e.button == 0)
                    {
                        if (windowRect.Contains(e.mousePosition))
                        {
                            Drag(e); 
                            e.Use();
                        }
                    }
                    break;
            }
        }

        public bool Contains(Vector2 pos)
        {
            return windowRect.Contains(pos);
        }

        public Vector2 GetWindowCentre()
        {
            return new Vector2(windowRect.position.x + windowRect.width / 2, windowRect.position.y + windowRect.height / 2);
        }

        private void ProcessNodeRendererEvents(Event e)
        {
            for (int i = EditorUI.NodeRenderers.Count - 1; i >= 0; i--)
            {
                EditorUI.NodeRenderers[i].ProcessEvents(e);
            }
        }

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New State"), false, () => EditorUI.StateMachineData.CreateNewState(mousePosition));

            // if clipboard is of type State
            //menu.AddItem(new GUIContent("Paste State"), false, () => throw new NotImplementedException());

            menu.ShowAsContext();
            EditorUI.ContextMenuIsOpen = true;
            GUI.changed = true;
        }

        private void Drag(Event e)
        {
            float dragDif = (dragStartPos - e.mousePosition).magnitude;
            if (dragDif > DRAG_THRESHOLD)
            {
                dragThresholdReached = true;
            }

            if (dragThresholdReached)
            {
                Vector2 drag = ScrollViewDrag;
                drag -= e.delta;
                drag.x = Mathf.Clamp(drag.x, 0, scrollView.width - windowRect.width);
                drag.y = Mathf.Clamp(drag.y, 0, scrollView.height - windowRect.height);

                ScrollViewDrag = drag;
                GUI.changed = true;
            }
        }

        private void DrawGrid(float gridSpacing, Color gridColor)
        {
            float zoomScale = 1;

            int widthDivs = Mathf.CeilToInt(SCROLL_VIEW_WIDTH / gridSpacing);
            int heightDivs = Mathf.CeilToInt(SCROLL_VIEW_HEIGHT / gridSpacing);

            Handles.BeginGUI();
            Handles.color = gridColor;

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i * zoomScale, -gridSpacing * zoomScale, 0), new Vector3(gridSpacing * i * zoomScale, SCROLL_VIEW_HEIGHT * zoomScale, 0f));
            }

            for (int i = 0; i < heightDivs; i++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing * zoomScale, gridSpacing * i * zoomScale, 0), new Vector3(SCROLL_VIEW_WIDTH * zoomScale, gridSpacing * i * zoomScale, 0f));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes()
        {
            for (int i = 0; i < EditorUI.NodeRenderers.Count; i++)
            {
                EditorUI.NodeRenderers[i].Draw();
            }
        }

        private void DrawTopToolbar()
        {
            float maxTabWidth = 150;

            GUIStyle style = new GUIStyle(EditorStyles.toolbar);
            style.padding = new RectOffset();

            EditorGUILayout.BeginHorizontal(style, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("New State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                Vector2 centre = GetWindowCentre();
                centre.x -= StateRenderer.WIDTH / 2;
                centre.y -= StateRenderer.HEADER_HEIGHT / 2;
                EditorUI.StateMachineData.CreateNewState(centre);
            }

            GUI.enabled = EditorUI.Selection != null && EditorUI.Selection is StateRenderer;
            if (GUILayout.Button("Delete State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                EditorUI.StateMachineData.RemoveState((EditorUI.Selection as StateRenderer).Node);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                (EditorUI.Selection as StateRenderer).Node.ClearActions();
            }
            GUI.enabled = true;

            GUILayout.Space(10);

            if (GUILayout.Button("Clear Machine", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                EditorUI.StateMachineData.ClearStateMachine();
                EditorUI.Refresh();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomToolbar()
        {
            GUIStyle toolbarStyle = new GUIStyle(EditorStyles.toolbar);
            toolbarStyle.padding = new RectOffset();

            EditorGUILayout.BeginHorizontal(toolbarStyle, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));

            GUI.enabled = false;
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.PrefixLabel((zoomScale * 100).ToString("#") + "%", EditorStyles.toolbarTextField);

            zoomScale = GUILayout.HorizontalSlider(zoomScale, ZOOM_SCALE_MIN, ZOOM_SCALE_MAX, GUILayout.MaxWidth(100));
            GUI.enabled = true;

            if (GUILayout.Button("Reset View", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
            {
                ResetView();
                windowRect.height = MIN_WINDOW_HEIGHT;
            }
          
            string stateMachineName = (EditorUI.StateMachineData is ScriptableObject) ? AssetDatabase.GetAssetPath(EditorUI.StateMachineData.SerializedObject) : string.Empty;
            EditorGUILayout.LabelField(stateMachineName.Replace(".asset", ""), NodeGUIStyles.CanvasBottomToolbarStyle);
            
            bool debug = EditorUI.ShowDebug;
            debug = GUILayout.Toggle(EditorUI.ShowDebug, "Debug", EditorStyles.toolbarButton, GUILayout.MaxWidth(50));
            if (debug != EditorUI.ShowDebug)
            {
                EditorUI.SetDebug(debug);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ResetView()
        {
            ScrollViewDrag = Vector2.zero;
            zoomScale = 1f;
        }

        public void FocusWindow(Vector2 position)
        {
            ScrollViewDrag = new Vector2(position.x - windowRect.width / 2, position.y - windowRect.height / 2);
        }
    }
}