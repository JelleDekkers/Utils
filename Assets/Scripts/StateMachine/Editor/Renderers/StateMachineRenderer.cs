using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.StateMachineData"/> window.
    /// </summary>
    public class StateMachineRenderer
    {
        public const float CANVAS_WIDTH = 1000;
        public const float CANVAS_HEIGHT = 1000;

        private const float GRID_LINE_INTERVAL = 50f;
        private const float WINDOW_HEIGHT = 400;

        public StateMachineData StateMachine { get; private set; }
        public Rect CanvasWindow { get; private set; }
        public Vector2 CanvasDrag { get; private set; }
        public StateMachineInspector Inspector { get; private set; }
        public bool ShowDebug { get; private set; }

        private readonly Action repaintFunc;
        private readonly Color backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        private readonly Color gridPrimaryColor = new Color(0, 0, 0, 0.18f);
        private readonly Color gridSecondaryColor = new Color(0, 0, 0, 0.1f);
        private readonly float gridPrimarySpacing = 20f;
        private readonly float gridSecondarySpacing = 100f;

        private ISelectable selectedObject;
        private List<StateRenderer> stateRenderers = new List<StateRenderer>();

        private Rect scrollView = new Rect();
        private float zoomScale = 100;

        public StateMachineRenderer(StateMachineData stateMachine, Action repaintFunc)
        {
            StateMachine = stateMachine;
            this.repaintFunc = repaintFunc;

            foreach(State state in stateMachine.States)
            {
                CreateNewStateRenderer(state);
            }

            Inspector = new StateMachineInspector(this);
        }

        public void OnInspectorGUI()
        {
            Event e = Event.current;

            DrawTopTabs();
            DrawCanvasWindow(e);
            DrawBottomTabs();
            Inspector.OnInspectorGUI(e);

            if (ShowDebug)
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

            if (selectedObject != null)
            {
                GUILayout.Label("selection " + selectedObject);

                if (selectedObject is StateRenderer)
                {   
                    GUILayout.Label("selection action count " + (selectedObject as StateRenderer).State.Actions.Count);
                    GUILayout.Label("selection rect " + (selectedObject as StateRenderer).Rect);
                    GUILayout.Label("selection rules " + (selectedObject as StateRenderer).State.RuleGroups.Count);
                    GUILayout.Label("entry state == selection " + (StateMachine.EntryState == (selectedObject as StateRenderer).State));
                    //GUILayout.Label("entry state ID " + StateMachine.EntryState.GetInstanceID());
                    //GUILayout.Label("state[0] ID " + StateMachine.States[0].GetInstanceID());
                }
            }
        }
        
        private void DrawCanvasWindow(Event e)
        {
            Rect temp = EditorGUILayout.BeginVertical(GUILayout.Height(WINDOW_HEIGHT));

            // hotfix to prevent strange issue where EditorGUILayout.BeginVertical returns zero every other frame
            if (temp != Rect.zero)
                scrollView = temp;
            
            Vector2 windowPos = EditorGUILayout.BeginScrollView(CanvasDrag, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none, GUILayout.Height(WINDOW_HEIGHT));
            CanvasWindow = new Rect(windowPos, scrollView.size); 

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            GUILayout.Box(GUIContent.none, GUILayout.Width(CANVAS_WIDTH), GUILayout.Height(CANVAS_HEIGHT));
            GUI.backgroundColor = oldColor;

            DrawGrid(gridPrimarySpacing, gridPrimaryColor); 
            DrawGrid(gridSecondarySpacing, gridSecondaryColor); 
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
                    //if (e.button == 1 && CanvasWindow.Contains(e.mousePosition)) // && not hovered above a state 
                    //{
                    //    ShowContextMenu(e);
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

        private void ShowContextMenu(Event e)
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

        private void DrawTopTabs()
        {
            float maxTabWidth = 150;
            float groupSpace = 10;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
            
            if(GUILayout.Button("New State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                CreateNewState();
            }

            GUI.enabled = selectedObject != null && selectedObject is StateRenderer;
            if (GUILayout.Button("Delete State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                RemoveState(selectedObject as StateRenderer);
            }

            if (GUILayout.Button("Reset State", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxTabWidth)))
            {
                ResetState((selectedObject as StateRenderer).State);
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
            string stateMachineName = AssetDatabase.GetAssetPath(StateMachine);
            EditorGUILayout.LabelField(stateMachineName.Replace(".asset", ""), style);

            GUILayout.Space(groupSpace);
            ShowDebug = GUILayout.Toggle(ShowDebug, "Debug", EditorStyles.toolbarButton, GUILayout.MaxWidth(50));

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
            return CreateNewState(GetWindowCentre());
        }

        private Vector2 GetWindowCentre()
        {
            return new Vector2(CanvasWindow.position.x + CanvasWindow.width / 2 - StateRenderer.WIDTH / 2, CanvasWindow.position.y + CanvasWindow.height / 2);
        }

        private State CreateNewState(Vector2 position)
        {
            string assetFilePath = AssetDatabase.GetAssetPath(StateMachine);
            State state = StateMachineEditorUtility.CreateObjectInstance<State>(assetFilePath);
            state.Rect.position = position;

            StateMachine.AddNewState(state);
            CreateNewStateRenderer(state);
            
            return state;
        }

        private StateRenderer CreateNewStateRenderer(State state)
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            stateRenderers.Add(stateRenderer);
            //Undo.RegisterCreatedObjectUndo(state, "Added state");

            return stateRenderer;
        }

        //private void OnSelectableObjectSelected(ISelectable inspectableObject)
        //{
        //    selectedObject = inspectableObject;

        //    if(selectedObject is IInspectable)
        //    Inspector.Inspect(selectedObject as IInspectable);
        //}

        //private void OnSelectableObjectDeSelected(ISelectable stateRenderer)
        //{
        //    selectedObject = null;
        //    if (Inspector.InspectedObject == stateRenderer)
        //    {
        //        Inspector.Clear();
        //    }
        //}

        private void ResetState(State state)
        {
            if(!(selectedObject is StateRenderer))
            {
                Debug.LogError(selectedObject + " is not of type " + typeof(StateRenderer).ToString());
            }

            (selectedObject as StateRenderer).ResetState();
        }

        public void RemoveState(StateRenderer renderer)
        {
            RemoveState(renderer.State);
        }

        public void RemoveState(State state)
        {
            bool isEntryState = StateMachine.EntryState == state;

            foreach (StateRenderer renderer in stateRenderers)
            {
                if (renderer.State == state)
                {
                    stateRenderers.Remove(renderer);
                    break;
                }
            }

            if (Inspector.InspectedObject == state)
            {
                Inspector.Clear();
            }

            StateMachine.RemoveState(state);
            UnityEngine.Object.DestroyImmediate(state, true);

            EditorUtility.SetDirty(StateMachine);
            AssetDatabase.SaveAssets();

            if (isEntryState && StateMachine.States.Count > 0)
            {
                StateMachine.SetEntryState(StateMachine.States[0]);
            }

            EditorUtility.SetDirty(StateMachine);
        }

        public void Refresh()
        {
            selectedObject = null;
            Inspector.Clear();

            foreach (State state in StateMachine.States)
            {
                CreateNewStateRenderer(state);
            }
        }

        private void ClearStateMachine()
        {
            Undo.RecordObject(StateMachine, "Clear Machine");

            StateMachine.States.Clear();
            stateRenderers.Clear();
            Inspector.Clear();
        }
    }
}