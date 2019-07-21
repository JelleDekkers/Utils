using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="StateMachine.StateMachineData"/> in an editor window.
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
        public StateMachineExecutor Executor { get; private set; }

        private readonly Action repaintFunc;

        public StateMachineEditorManager(StateMachineData stateMachine, Action repaintFunc, StateMachineExecutor executor = null)
        {
            StateMachineData = stateMachine;
            Inspector = new StateMachineInspector(this);
            CanvasRenderer = new StateMachineCanvasRenderer(this);
            this.repaintFunc = repaintFunc;
            this.Executor = executor;

            StateRenderers = new List<StateRenderer>();
            foreach (State state in stateMachine.States)
            {
                CreateNewStateRenderer(state);
            }

            StateMachineEditorUtility.StateAddedEvent += OnStateAddedEvent;
            StateMachineEditorUtility.StateRemovedEvent += OnStateRemovedEvent;
            StateMachineEditorUtility.StateMachineClearedEvent += OnStateMachineClearedEvent;
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
                    GUILayout.Label("selection action count " + (Selection as StateRenderer).State.Actions.Count);
                    GUILayout.Label("selection rect " + (Selection as StateRenderer).Rect);
                    GUILayout.Label("selection rules " + (Selection as StateRenderer).State.RuleGroups.Count);
                    GUILayout.Label("entry state == selection " + (StateMachineData.EntryState == (Selection as StateRenderer).State));
                }
            }
        }

        private void OnStateMachineClearedEvent(StateMachineData stateMachine)
        {
            if(StateMachineData == stateMachine)
            {
                StateRenderers.Clear();
            }
        }

        private void OnStateRemovedEvent(StateMachineData stateMachine, State state)
        {
           if(StateMachineData == stateMachine)
            {
                RemoveStateRenderer(state);
            }
        }

        private void RemoveStateRenderer(State state)
        {
            foreach (StateRenderer renderer in StateRenderers)
            {
                if (renderer.State == state)
                {
                    StateRenderers.Remove(renderer);

                    if (Selection == renderer as ISelectable )
                    {
                        Deselect(renderer as ISelectable);
                    }
                    break;
                }
            }
        }

        private void OnStateAddedEvent(StateMachineData stateMachine, State state)
        {
            if (StateMachineData == stateMachine)
            {
                CreateNewStateRenderer(state);
            }
        }

        private void CreateNewStateRenderer(State state)
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            StateRenderers.Insert(0, stateRenderer);
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

            if (Inspector.InspectedObject != null)
            {
                Inspector.Refresh();
            }
        }

        /// <summary>
        /// Reorders renderer to the the bottom of the states list, this way <see cref="StateRenderer.ProcessEvents(Event)"/> is called first and the window will be drawn on top
        /// </summary>
        /// <param name="renderer"></param>
        public void ReorderStateRendererToBottom(StateRenderer renderer)
        {
            StateRenderers.Remove(renderer);
            StateRenderers.Add(renderer);
        }

        public void Dispose()
        {
            StateMachineEditorUtility.StateAddedEvent -= OnStateAddedEvent;
            StateMachineEditorUtility.StateRemovedEvent -= OnStateRemovedEvent;
            StateMachineEditorUtility.StateMachineClearedEvent -= OnStateMachineClearedEvent;
        }
    }
}