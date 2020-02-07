using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.Core.Flow.Inspector;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for rendering a single <see cref="Flow.StateMachineScriptableObjectData"/>.
    /// </summary>
    public class StateMachineLayerRenderer
    {
        public ISelectable CurrentSelected { get; private set; }
        public IStateMachineData StateMachineData { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public List<INodeRenderer<State>> NodeRenderers { get; private set; }
        public StateMachineCanvasRenderer CanvasRenderer { get; private set; }
        public StateMachineInspector Inspector { get; private set; }
        public bool ContextMenuIsOpen { get; set; }
        public bool ShowDebug { get; private set; }

        private readonly Action repaintEvent;

        public StateMachineLayerRenderer(IStateMachineData data, Action repaintEvent, StateMachine stateMachine = null)
        {
            StateMachineData = data;
            this.StateMachine = stateMachine;
            this.repaintEvent = repaintEvent;

            Inspector = new StateMachineInspector(this);
            CanvasRenderer = new StateMachineCanvasRenderer(this);
            NodeRenderers = new List<INodeRenderer<State>>();

            foreach (State state in data.States)
            {
                CreateNewNodeRenderer(state);
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
                repaintEvent();
            }
        }

        public bool IsStateAtPosition(Vector2 position, out StateRenderer result)
        {
            foreach(StateRenderer renderer in NodeRenderers)
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
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Drag " + CanvasRenderer.ScrollViewDrag);
            EditorGUILayout.LabelField("window size " + CanvasRenderer.windowRect.size);
            EditorGUILayout.LabelField("mouse pos " + e.mousePosition);
            EditorGUILayout.LabelField("state count " + StateMachineData.States.Count);

            if (StateMachineData.EntryState != null)
            {
                EditorGUILayout.LabelField("entry state " + StateMachineData.EntryState.Title);
            }

            if (CurrentSelected != null)
            {
                EditorGUILayout.LabelField("selection " + CurrentSelected);

                if (CurrentSelected is StateRenderer)
                {
                    EditorGUILayout.LabelField("selection pos " + (CurrentSelected as StateRenderer).Node.Position);
                    EditorGUILayout.LabelField("selection rect " + (CurrentSelected as StateRenderer).Rect);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnStateMachineClearedEvent(IStateMachineData stateMachine)
        {
            if(StateMachineData == stateMachine)
            {
                foreach(StateRenderer renderer in NodeRenderers)
                {
                    renderer.OnDestroy();
                }

                NodeRenderers.Clear();
            }
        }

        private void OnStateRemovedEvent(IStateMachineData stateMachine, State state)
        {
            if(StateMachineData == stateMachine)
            {
                RemoveStateRenderer(state);
            }
        }

        private void RemoveStateRenderer(State state)
        {
            foreach (StateRenderer renderer in NodeRenderers)
            {
                if (renderer.Node == state)
                {
                    NodeRenderers.Remove(renderer);
                    renderer.OnDestroy();

                    if (CurrentSelected != null && CurrentSelected == renderer as ISelectable )
                    {
                        Deselect(renderer as ISelectable);
                    }
                    break;
                }
            }
        }

        private void OnStateAddedEvent(IStateMachineData stateMachine, State state)
        {
            if (StateMachineData == stateMachine)
            {
                CreateNewNodeRenderer(state);
            }
        }

        private void CreateNewNodeRenderer(State state) 
        {
            StateRenderer stateRenderer = new StateRenderer(state, this);
            NodeRenderers.Insert(0, stateRenderer);
        }

        public void Select(ISelectable selectable)
        {
            if (CurrentSelected != selectable)
            {
                CurrentSelected = selectable;

                IInspectable inspectable = selectable as IInspectable;
                if (inspectable != null)
                {
                    Inspector.Inspect(inspectable.CreateInspectorBehaviour());
                }
            }
        }

        public void Deselect(ISelectable selectable)
        {
            if(CurrentSelected != null && CurrentSelected == selectable)
            {
                CurrentSelected = null;
                Inspector.Clear();
            }
        }

        public void Refresh()
        {
            CurrentSelected = null;
            NodeRenderers.Clear();
            Inspector.Refresh();

            foreach (State state in StateMachineData.States)
            {
                CreateNewNodeRenderer(state);
            }
        }

        public void SetDebug(bool debug)
        {
            ShowDebug = debug;
            Inspector.Refresh();
        }

        /// <summary>
        /// Reorders renderer to the the bottom of the states list, this way <see cref="StateRenderer.ProcessEvents(Event)"/> is called first and the window will be drawn on top
        /// </summary>
        /// <param name="renderer"></param>
        public void ReorderNodeRendererToBottom(StateRenderer renderer)
        {
            NodeRenderers.Remove(renderer);
            NodeRenderers.Add(renderer);
        }

        public void OnDestroy()
        {
            foreach(StateRenderer renderer in NodeRenderers)
            {
                renderer.OnDestroy();
            }

            Inspector.Clear();

            StateMachineEditorUtility.StateAddedEvent -= OnStateAddedEvent;
            StateMachineEditorUtility.StateRemovedEvent -= OnStateRemovedEvent;
            StateMachineEditorUtility.StateMachineClearedEvent -= OnStateMachineClearedEvent;
        }
    }
}