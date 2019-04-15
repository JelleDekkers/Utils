using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.StateMachine"/> window.
    /// </summary>
    public class StateMachineRenderer
    {
        public const float CANVAS_WIDTH = 1000;
        public const float CANVAS_HEIGHT = 1000;

        private const float GRID_LINE_INTERVAL = 50f;
        private const float WINDOW_HEIGHT = 400;

        public StateMachine StateMachine { get; private set; }
        public Rect CanvasWindow { get; private set; }
        public Vector2 CanvasDrag { get; private set; }

        private readonly Color backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        private readonly Color gridPrimaryColor = new Color(0, 0, 0, 0.18f);
        private readonly Color gridSecondaryColor = new Color(0, 0, 0, 0.1f);
        private readonly float gridPrimarySpacing = 20f;
        private readonly float gridSecondarySpacing = 100f;

        private StateInspector stateInspector;
        private Action repaintFunc;
        private StateRenderer selectedStateRenderer;
        private List<StateRenderer> stateRenderers = new List<StateRenderer>();
        private List<LinkRenderer> linkRenderers = new List<LinkRenderer>();

        private float zoomScale = 100;
        private bool showDebugUI;

        public StateMachineRenderer(StateMachine stateMachine, Action repaintFunc)
        {
            StateMachine = stateMachine;
            this.repaintFunc = repaintFunc;

            foreach(State state in stateMachine.States)
            {
                CreateNewStateRenderer(state);
            }

            stateInspector = new StateInspector(stateMachine);
        }

        public void OnInspectorGUI()
        {
            Event e = Event.current;

            DrawTopTabs();
            DrawCanvasWindow(e);
            DrawBottomTabs();
            stateInspector.OnInspectorGUI(e);

            showDebugUI = GUILayout.Toggle(showDebugUI, "Show Debug Info");
            if (showDebugUI)
            {
                DebugInfo(e);
            }

            if (GUI.changed)
            {
                repaintFunc();
            }
        }

        public bool IsStateAtPosition(Vector2 position, out StateRenderer result)
        {
            foreach(StateRenderer renderer in stateRenderers)
            {
                if(renderer.Rect.Contains(position))
                {
                    result = renderer;
                    return true;
                }
            }

            result = null;
            return false;
        }

        private void DebugInfo(Event e)
        {
            GUILayout.Label("window " + CanvasWindow);
            GUILayout.Label("Canvas position " + CanvasWindow.position);
            GUILayout.Label("pan " + CanvasDrag);

            GUILayout.Label("mouse pos " + e.mousePosition);
            GUILayout.Label("state count " + StateMachine.States.Count);
            GUILayout.Label("entry state " + StateMachine.EntryState.Title); 

            if (selectedStateRenderer != null)
            {
                GUILayout.Label("selection " + selectedStateRenderer);
                GUILayout.Label("selection action count " + selectedStateRenderer.State.Actions.Count);
                GUILayout.Label("selection rect " + selectedStateRenderer.Position);
                GUILayout.Label("selection rules " + selectedStateRenderer.State.Rules.Count);
                GUILayout.Label("entry state == selection " + (StateMachine.EntryState == selectedStateRenderer.State));
                //GUILayout.Label("entry state ID " + StateMachine.EntryState.GetInstanceID());
                //GUILayout.Label("state[0] ID " + StateMachine.States[0].GetInstanceID());
            }
        }
        
        private void DrawCanvasWindow(Event e)
        {
            CanvasWindow = EditorGUILayout.BeginVertical(GUILayout.Height(WINDOW_HEIGHT));

            Vector2 windowPos = EditorGUILayout.BeginScrollView(CanvasDrag, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Height(WINDOW_HEIGHT));
            CanvasWindow = new Rect(windowPos, CanvasWindow.size); // reset canvas window position to ensure it's 0,0 is set at the scrollView origin

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            GUILayout.Box(GUIContent.none, GUILayout.Width(CANVAS_WIDTH), GUILayout.Height(CANVAS_HEIGHT));
            GUI.backgroundColor = oldColor;

            DrawGrid(gridPrimarySpacing, gridPrimaryColor); 
            DrawGrid(gridSecondarySpacing, gridSecondaryColor); 
            DrawConnections();
            DrawStates();
            ProcessNodeEvents(e);
            ProcessEvents(e);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    //if (e.button == 0)
                    //{
                    //    ClearConnectionSelection();
                    //}

                    //if (e.button == 1 && CanvasWindow.Contains(e.mousePosition))
                    //{
                    //    ShowAddStateContextMenu(e);
                    //}
                    //break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (CanvasWindow.Contains(e.mousePosition))
                        {
                            Drag(e.delta);
                            e.Use();
                        }
                    }
                    else if(e.button == 1)
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

        private void ProcessNodeEvents(Event e)
        {
            for (int i = stateRenderers.Count - 1; i >= 0; i--)
            {
                bool guiChanged = stateRenderers[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }

        private void ShowAddStateContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New State"), false, () => CreateNewState(e.mousePosition));
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
            for (int i = 0; i < stateRenderers.Count; i++)
            {
                stateRenderers[i].Draw();
            }
        }

        private void DrawConnections()
        {
            for (int i = 0; i < linkRenderers.Count; i++)
            {
                //linkRenderers[i].Draw();
            }
        }

        private void DrawTopTabs()
        {
            float maxTabWidth = 150;
            float groupSpace = 10;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
            
            if(GUILayout.Button("New State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                CreateNewState();
            }

            GUI.enabled = selectedStateRenderer != null;
            if (GUILayout.Button("Delete State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                DeleteState(selectedStateRenderer);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                ResetState(selectedStateRenderer.State);
            }
            GUI.enabled = true;

            GUILayout.Space(groupSpace);

            if (GUILayout.Button("Clear Machine", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                ClearStateMachine();

            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomTabs()
        {
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

            if (GUILayout.Button("Reset View", EditorStyles.toolbarButton, GUILayout.MaxWidth(100)))
            {
                ResetView();
            }

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleRight;
            string stateMachineName = AssetDatabase.GetAssetPath(StateMachine);
            EditorGUILayout.LabelField(stateMachineName.Replace(".asset", ""), style);

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

        private State CreateNewState()
        {
            // TODO: fix:
            Vector2 position = new Vector2(CanvasWindow.position.x + CanvasWindow.height / 2 + StateRenderer.WIDTH, CanvasWindow.position.y + CanvasWindow.height / 2);
            return CreateNewState(position);
        }

        private State CreateNewState(Vector2 mousePosition)
        {
            string assetFilePath = AssetDatabase.GetAssetPath(StateMachine);
            State state = StateMachineEditorUtility.CreateObjectInstance<State>(assetFilePath);
            state.Position = mousePosition;

            StateMachine.AddNewState(state);
            CreateNewStateRenderer(state);
            
            return state;
        }

        private StateRenderer CreateNewStateRenderer(State state)
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            stateRenderers.Add(stateRenderer);
            //Undo.RegisterCreatedObjectUndo(state, "Added state");

            stateRenderer.DeleteEvent += DeleteState;
            stateRenderer.SelectedEvent += OnStateRendererSelected;
            stateRenderer.DeselectedEvent += OnStateRendererDeselected;

            return stateRenderer;
        }

        private void DeleteState(StateRenderer stateRenderer)
        {
            bool isEntryState = StateMachine.EntryState == stateRenderer.State;
            StateMachine.RemoveState(stateRenderer.State);
            stateRenderers.Remove(stateRenderer);
            //UnityEditor.Undo.DestroyObjectImmediate(state);

            EditorUtility.SetDirty(StateMachine);
            AssetDatabase.SaveAssets();

            if (selectedStateRenderer == stateRenderer)
            {
                selectedStateRenderer = null;
                stateInspector.Show(null);
            }

            if(isEntryState && StateMachine.States.Count > 0)
            {
                StateMachine.SetEntryState(StateMachine.States[0]);
            }

            stateRenderer.DeleteEvent -= DeleteState;
            stateRenderer.SelectedEvent -= OnStateRendererSelected;
            stateRenderer.DeselectedEvent -= OnStateRendererDeselected;

            EditorUtility.SetDirty(StateMachine);
        }

        private void OnStateRendererSelected(StateRenderer stateRenderer)
        {
            selectedStateRenderer = stateRenderer;
            stateInspector.Show(stateRenderer.State);
        }

        private void OnStateRendererDeselected(StateRenderer stateRenderer)
        {
            selectedStateRenderer = null;
            if (stateInspector.State == stateRenderer.State)
            {
                stateInspector.Show(null);
            }
        }

        private void ResetState(State state)
        {
            for (int i = state.Actions.Count - 1; i >= 0; i--)
            {
                state.RemoveAction(state.Actions[i]);
                ScriptableObject.DestroyImmediate(state.Actions[i], true);
            }

            for (int i = state.Rules.Count - 1; i >= 0; i--)
            {
                state.RemoveRule(state.Rules[i]);
            }

            selectedStateRenderer.Reset();
            stateInspector.Refresh();
        }

        private void ClearStateMachine()
        {
            Undo.RecordObject(StateMachine, "Clear Machine");

            stateRenderers.Clear();
            StateMachine.States.Clear();
            linkRenderers.Clear();
            stateInspector.Show(null);
        }
    }
}