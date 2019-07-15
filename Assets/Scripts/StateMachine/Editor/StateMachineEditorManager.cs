using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.StateMachineData"/> window.
    /// </summary>
    public class StateMachineEditorManager : IDisposable
    {
        public ISelectable Selection { get; private set; }
        public StateMachineData StateMachineData { get; private set; }
        public List<StateRenderer> StateRenderers { get; private set; }
        public StateMachineCanvasRenderer CanvasRenderer { get; private set; }
        public StateMachineInspector Inspector { get; private set; }
        public bool ContextMenuIsOpen { get; set; }

        public bool ShowDebug { get; private set; }

        private readonly Action repaintFunc;

        public StateMachineEditorManager(StateMachineData stateMachine, Action repaintFunc)
        {
            StateMachineData = stateMachine;
            Inspector = new StateMachineInspector(this);
            CanvasRenderer = new StateMachineCanvasRenderer(this);
            this.repaintFunc = repaintFunc;

            StateRenderers = new List<StateRenderer>();
            foreach (State state in stateMachine.States)
            {
                CreateNewStateRenderer(state);
            }

            StateMachineEditorUtility.StateAddedEvent += CreateNewStateRenderer;
            StateMachineEditorUtility.StateRemovedEvent += OnStateRemovedEvent;
        }

        public void OnInspectorGUI()
        {
            Event e = Event.current;
            CanvasRenderer.OnInspectorGUI(e);
            Inspector.OnInspectorGUI(e);

            if (ShowDebug)
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
                    GUILayout.Label("selection action count " + (Selection as StateRenderer).DataObject.Actions.Count);
                    GUILayout.Label("selection rect " + (Selection as StateRenderer).Rect);
                    GUILayout.Label("selection rules " + (Selection as StateRenderer).DataObject.RuleGroups.Count);
                    GUILayout.Label("entry state == selection " + (StateMachineData.EntryState == (Selection as StateRenderer).DataObject));
                }
            }
        }

        private void OnStateRemovedEvent(State state)
        {
            if (Selection == state as ISelectable)
            {
                Deselect(state as ISelectable);
            }

            foreach (StateRenderer renderer in StateRenderers)
            {
                if (renderer.DataObject == state)
                {
                    StateRenderers.Remove(renderer);
                    break;
                }
            }
        }

        private void CreateNewStateRenderer(State state)
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            StateRenderers.Add(stateRenderer);
        }

        public void Select(ISelectable selectable)
        {
            if (Selection != selectable)
            {
                Selection = selectable;
            }
        }

        public void Deselect(ISelectable selectable)
        {
            if(Selection == selectable)
            {
                Selection = null;
                Inspector.Clear();
            }
        }

        public void Refresh(Action onDone = null)
        {
            Selection = null;

            foreach (State state in StateMachineData.States)
            {
                CreateNewStateRenderer(state);
            }

            onDone?.Invoke();
        }

        public void SetDebug(bool debug)
        {
            ShowDebug = debug;
            Inspector.Refresh();
        }

        /// <summary>
        /// Reorders renderer to the the bottom of the states list, this way <see cref="StateRenderer.ProcessEvents(Event)"/> is called last and the window will be drawn on top
        /// </summary>
        /// <param name="renderer"></param>
        public void ReorderStateRendererToBottom(StateRenderer renderer)
        {
            StateRenderers.Remove(renderer);
            StateRenderers.Add(renderer);
        }

        public void Dispose()
        {
            StateMachineEditorUtility.StateAddedEvent -= CreateNewStateRenderer;
            StateMachineEditorUtility.StateRemovedEvent -= OnStateRemovedEvent;
        }
    }
}