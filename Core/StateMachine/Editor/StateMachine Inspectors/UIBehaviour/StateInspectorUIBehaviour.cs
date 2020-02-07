using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(State))]
    public class StateInspectorUIBehaviour : IInspectorUIBehaviour
    {
        private const string ACTIONS_PROPERTY_NAME = "TemplateActions"; 
        private const string STATES_PROPERTY_NAME = "states";

        private readonly IStateMachineData stateMachineData;
        private readonly State state;

        private SerializedObject serializedStateMachine;
        private SerializedProperty actionsProperty;

        public StateInspectorUIBehaviour(IStateMachineData stateMachineData, State state)
        {
            this.stateMachineData = stateMachineData;
            this.state = state;

            Refresh();
        }

        public void Refresh()
        {
            serializedStateMachine = new SerializedObject(stateMachineData.SerializedObject);

            int stateIndex = stateMachineData.States.IndexOf(state);
            SerializedProperty stateProperty = (serializedStateMachine.targetObject is StateMachineMonoBehaviour)
                ? serializedStateMachine.FindProperty("Data").FindPropertyRelative(STATES_PROPERTY_NAME).GetArrayElementAtIndex(stateIndex)
                : serializedStateMachine.FindProperty(STATES_PROPERTY_NAME).GetArrayElementAtIndex(stateIndex);
            actionsProperty = stateProperty.FindPropertyRelative(ACTIONS_PROPERTY_NAME);
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawHeader("State Actions");

            if (actionsProperty.arraySize > 0)
            {
                InspectorUIUtility.DrawHorizontalLine();
            }

            for (int i = 0; i < actionsProperty.arraySize; i++)
            {
                InspectorUIUtility.DrawArrayPropertyField(actionsProperty, i, OnContextMenuButtonPressed);
            }

            EditorGUILayout.EndVertical();
        }

        protected void DrawHeader(string title)
        {
            GUIStyle buttonStyle = new GUIStyle("button");

            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.DelayedTextField(state.Title);

            if (newName != state.Title)
            {
                Undo.RecordObject(stateMachineData.SerializedObject, "Change State Name");
                state.Title = newName;
                EditorUtility.SetDirty(serializedStateMachine.targetObject);
            }

            GUI.enabled = stateMachineData.EntryState != state;
            string tooltip = (GUI.enabled) ? "Make entry state" : "Is already entry state";
            if (GUILayout.Button(new GUIContent("Entry State", tooltip), buttonStyle, GUILayout.Width(90)))
            {
                OnSetEntryStateButtonPressedEvent();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            InspectorUIUtility.DrawHeader(title, () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent, "Add new StateAction"));
        }

        private void OnSetEntryStateButtonPressedEvent()
        {
            Undo.RecordObject(serializedStateMachine.targetObject, "Set Entry State");
            stateMachineData.SetEntryState(state);
            EditorUtility.SetDirty(serializedStateMachine.targetObject);
        }

        private void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(StateAction), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            state.AddStateAction(type, stateMachineData);
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
        }

        private void OnDeleteButtonPressed(ContextMenu.Result result)
        {
            state.RemoveStateAction(state.TemplateActions[result.Index], stateMachineData);
            Refresh();
        }

        //private void OnResetButtonPressed(ContextMenu.Result result)
        //{
        //    state.TemplateActions[result.Index].Reset(state, editorUI.StateMachineData);
        //    Refresh();
        //}

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            Undo.RegisterCompleteObjectUndo(stateMachineData.SerializedObject, "Reorder Actions");

            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < state.TemplateActions.Count)
            {
                state.TemplateActions.ReorderItem(result.Index, newIndex);
            }

            Refresh();
        }
    }
}