using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(State))]
    public class StateInspectorUI : IInspectorUIBehaviour
    {
        private const string PROPERTY_NAME = "TemplateActions";

        private StateMachineLayerRenderer editorUI;
        private State state;
        private SerializedObject serializedState;

        public StateInspectorUI(StateMachineLayerRenderer editorUI, State state)
        {
            this.editorUI = editorUI;
            this.state = state;

            Init();
        }

        private void Init()
        {
            serializedState = new SerializedObject(state);
        }

        public void Refresh()
        {
            Init();
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawHeader("State Actions");
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawPropertyFields(serializedState, PROPERTY_NAME, OnContextMenuButtonPressed);
            EditorGUILayout.EndVertical();
        }

        protected void DrawHeader(string title)
        {
            GUIStyle buttonStyle = new GUIStyle("button");
            buttonStyle.margin.top = 0;

            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.DelayedTextField(state.Title);

            if (newName != state.Title)
            {
                //Undo.RecordObject(editorUI.StateMachineData, "Change State Name");
                Undo.RecordObject(state, "Change State Name");
                state.Title = newName;
                EditorUtility.SetDirty(serializedState.targetObject);
            }

            GUI.enabled = editorUI.StateMachineData.EntryState != state;
            if (GUILayout.Button("Entry State", buttonStyle, GUILayout.Width(90)))
            {
                OnSetEntryStateButtonPressedEvent();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            InspectorUIUtility.DrawHeader(title, () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent));
        }

        private void OnSetEntryStateButtonPressedEvent()
        {
            editorUI.StateMachineData.SetEntryState(state);
        }

        private void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(StateAction), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            state.AddStateAction(type, editorUI.StateMachineData);
            Refresh();
        }

        private void OnContextMenuButtonPressed(object o)
        {
            ContextMenu.Result result = (ContextMenu.Result)o;

            switch (result.Command)
            {
                case ContextMenu.Command.EditScript:
                    OnEditScriptButtonPressed(result);
                    break;
                case ContextMenu.Command.MoveUp:
                    OnReorderButtonPressed(result, ContextMenu.ReorderDirection.Up);
                    break;
                case ContextMenu.Command.MoveDown:
                    OnReorderButtonPressed(result, ContextMenu.ReorderDirection.Down);
                    break;
                //case ContextMenu.Command.Reset:
                //    OnResetButtonPressed(result);
                //    break;
                case ContextMenu.Command.Delete:
                    OnDeleteButtonPressed(result);
                    break;
            }
        }

        private void OnEditScriptButtonPressed(ContextMenu.Result result)
        {
            InspectorUIUtility.OpenScript(result.Obj);
            editorUI.Inspector.Refresh();
        }

        private void OnDeleteButtonPressed(ContextMenu.Result result)
        {
            state.RemoveStateAction(state.TemplateActions[result.Index]);
            Refresh();
        }

        //private void OnResetButtonPressed(ContextMenu.Result result)
        //{
        //    state.TemplateActions[result.Index].Reset(state, editorUI.StateMachineData);
        //    Refresh();
        //}

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            Undo.RegisterCompleteObjectUndo(state, "Reorder Actions");

            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < state.TemplateActions.Count)
            {
                state.TemplateActions.ReorderItem(result.Index, newIndex);
            }

            Refresh();
        }
    }
}