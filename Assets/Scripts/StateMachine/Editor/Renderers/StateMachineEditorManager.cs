using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.StateMachineData"/> window.
    /// </summary>
    public class StateMachineEditorManager
    {
        public ISelectable Selection { get; private set; }
        public StateMachineData StateMachineData { get; private set; }
        public List<StateRenderer> StateRenderers { get; private set; }
        public StateMachineCanvasRenderer CanvasRenderer { get; private set; }

        public bool debug;

        private StateMachineInspector inspector;
        private readonly Action repaintFunc;

        public StateMachineEditorManager(StateMachineData stateMachine, Action repaintFunc)
        {
            StateMachineData = stateMachine;
            inspector = new StateMachineInspector(this);
            CanvasRenderer = new StateMachineCanvasRenderer(this);
            this.repaintFunc = repaintFunc;

            StateRenderers = new List<StateRenderer>();
            foreach(State state in stateMachine.States)
            {
                CreateNewStateRenderer(state);
            }
        }

        public void OnInspectorGUI()
        {
            Event e = Event.current;
            CanvasRenderer.OnInspectorGUI(e);
            inspector.OnInspectorGUI(e);
            CanvasRenderer.ProcessEvents(e);

            if (debug)
            {
                DrawDebugInfo(e);
            }

            if (GUI.changed)
            {
                repaintFunc();
            }
        }

        public bool IsStateAtPosition(Vector2 position, out StateRenderer result)
        {
            foreach(StateRenderer renderer in StateRenderers)
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

        private void DrawDebugInfo(Event e)
        {
            GUILayout.Label("mouse pos " + e.mousePosition);
            GUILayout.Label("state count " + StateMachineData.States.Count);
            GUILayout.Label("entry state " + StateMachineData.EntryState.Title);
            GUILayout.Label("is inside canvas " + CanvasRenderer.Contains(e.mousePosition));

            if (Selection != null)
            {
                GUILayout.Label("selection " + Selection);

                if (Selection is StateRenderer)
                {   
                    GUILayout.Label("selection action count " + (Selection as StateRenderer).State.Actions.Count);
                    GUILayout.Label("selection rect " + (Selection as StateRenderer).Rect);
                    GUILayout.Label("selection rules " + (Selection as StateRenderer).State.RuleGroups.Count);
                    GUILayout.Label("entry state == selection " + (StateMachineData.EntryState == (Selection as StateRenderer).State));
                }
            }
        }

        public State CreateNewState()
        {
            return CreateNewState(CanvasRenderer.GetWindowCentre());
        }

        public State CreateNewState(Vector2 position)
        {
            // TODO: undo
            string assetFilePath = AssetDatabase.GetAssetPath(StateMachineData);
            State state = StateMachineEditorUtility.CreateObjectInstance<State>(assetFilePath);
            state.Rect.position = position;

            StateMachineData.AddNewState(state);
            CreateNewStateRenderer(state);
            
            return state;
        }

        private StateRenderer CreateNewStateRenderer(State state)
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            StateRenderers.Add(stateRenderer);
            //Undo.RegisterCreatedObjectUndo(state, "Added state");

            return stateRenderer;
        }

        public void ResetState(State state)
        {
            // TODO: undo
            (Selection as StateRenderer).ResetState();
        }

        public void RemoveState(StateRenderer renderer)
        {
            // TODO: undo
            RemoveState(renderer.State);
        }

        public void RemoveState(State state)
        {
            // TODO: undo
            if(Selection == state as ISelectable)
            {
                Deselect(state as ISelectable);
            }

            bool isEntryState = StateMachineData.EntryState == state;

            foreach (StateRenderer renderer in StateRenderers)
            {
                if (renderer.State == state)
                {
                    StateRenderers.Remove(renderer);
                    break;
                }
            }

            StateMachineData.RemoveState(state);
            UnityEngine.Object.DestroyImmediate(state, true);

            EditorUtility.SetDirty(StateMachineData);
            AssetDatabase.SaveAssets();

            if (isEntryState && StateMachineData.States.Count > 0)
            {
                StateMachineData.SetEntryState(StateMachineData.States[0]);
            }

            EditorUtility.SetDirty(StateMachineData);
        }

        public void Select(ISelectable selectable)
        {
            Selection = selectable;

            if(selectable is IInspectable)
            {
                inspector.Inspect(selectable as IInspectable);
            }

            if(selectable is StateRenderer)
            {
                ReorderStateRendererToTop(selectable as StateRenderer);
            }
        }

        public void Deselect(ISelectable selectable)
        {
            if(Selection == selectable)
            {
                Selection = null;
                inspector.Clear();
            }
        }

        public void Refresh()
        {
            Selection = null;
            if (Selection != null)
            {
                inspector.Refresh();
            }

            foreach (State state in StateMachineData.States)
            {
                CreateNewStateRenderer(state);
            }
        }

        private void ReorderStateRendererToTop(StateRenderer renderer)
        {
            StateRenderers.Remove(renderer);
            StateRenderers.Add(renderer);
        }

        public void ClearStateMachine()
        {
            Undo.RecordObject(StateMachineData, "Clear Machine");

            StateMachineData.States.Clear();
            StateRenderers.Clear();
            inspector.Clear();
        }
    }
}