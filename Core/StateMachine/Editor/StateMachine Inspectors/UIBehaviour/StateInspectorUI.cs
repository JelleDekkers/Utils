using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(State))]
    public class StateInspectorUI : IInspectorUIBehaviour
    {
        private const string ACTIONS_PROPERTY_NAME = "Actions";

        private StateMachineLayerRenderer editorUI;
        private State state;
        private SerializedObject serializedStateMachine;

        public StateInspectorUI(StateMachineLayerRenderer editorUI, State state)
        {
            this.editorUI = editorUI;
            this.state = state;

            Refresh();
        }

        public void Refresh()
        {
            serializedStateMachine = new SerializedObject(editorUI.StateMachineData.SerializedObject);
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawHeader("State Actions");
            InspectorUIUtility.DrawHorizontalLine();
            //InspectorUIUtility.DrawPropertyFields(serializedStateMachine, ACTIONS_PROPERTY_NAME, OnContextMenuButtonPressed);


            SerializedProperty statesProperty = serializedStateMachine.FindProperty("states");
            int index = editorUI.StateMachineData.States.IndexOf(state);
            //InspectorUIUtility.DrawPropertyArrayField(statesProperty, index);
            DrawStateActions(serializedStateMachine, state, index);

            EditorGUILayout.EndVertical();
        }

        private void DrawStateActions(SerializedObject stateMachineData, State state, int stateIndex)
        {
            SerializedProperty stateProperty = stateMachineData.FindProperty("states").GetArrayElementAtIndex(stateIndex);
            SerializedProperty actionsProperty = stateProperty.FindPropertyRelative("Actions");

            Rect header = EditorGUILayout.BeginHorizontal(NodeGUIStyles.InspectorStyle);
            header.x += 3;
            //actionsProperty.isExpanded = EditorGUILayout.Foldout(actionsProperty.isExpanded, GUIContent.none, true);

            EditorGUI.LabelField(header, actionsProperty.displayName, NodeGUIStyles.FieldNameLabelStyle);
            //if (contextMenuPressedCallback != null)
            //{
            //    DrawContextMenuDropdown(index, actionsProperty.objectReferenceValue as ScriptableObject, contextMenuPressedCallback);
            //}
            EditorGUILayout.EndHorizontal();

            if (!actionsProperty.isExpanded) { return; }
            EditorGUI.indentLevel++;


            for (int i = 0; i < actionsProperty.arraySize; i++)
            {
                InspectorUIUtility.DrawPropertyArrayField(actionsProperty, i);
                //SerializedProperty property = actionsProperty.GetArrayElementAtIndex(i);
                //EditorGUILayout.PropertyField(property, true);
                //DrawField(property, actionsProperty.serializedObject);
            }

            return;

            SerializedObject obj = stateMachineData;
            if (obj == null) { return; }


            SerializedProperty field = obj.GetIterator();
            field.NextVisible(true);

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUI.indentLevel++;

            field = obj.GetIterator();
            field.NextVisible(true);

            EditorGUIUtility.labelWidth = header.width / 2.5f;
            while (field.NextVisible(false))
            {
                try
                {
                    EditorGUILayout.PropertyField(field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting causing a StackOverflowException, avoid using the same object inside a nested structure.");
                }
            }

            obj.ApplyModifiedProperties();

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void DrawField(SerializedProperty field, SerializedObject targetObject)
        {
            EditorGUI.indentLevel++;

            int index = 0;
            field = targetObject.GetIterator();
            field.NextVisible(true);

            while (field.NextVisible(false))
            {
                try
                {
                    EditorGUILayout.PropertyField(field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same object iside a nested structure.");
                }

                index++;
            }

            targetObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
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
                Undo.RecordObject(editorUI.StateMachineData.SerializedObject, "Change State Name");
                state.Title = newName;
                EditorUtility.SetDirty(serializedStateMachine.targetObject);
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
            state.RemoveStateAction(state.Actions[result.Index]);
            Refresh();
        }

        //private void OnResetButtonPressed(ContextMenu.Result result)
        //{
        //    state.TemplateActions[result.Index].Reset(state, editorUI.StateMachineData);
        //    Refresh();
        //}

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            Undo.RegisterCompleteObjectUndo(editorUI.StateMachineData.SerializedObject, "Reorder Actions");

            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < state.Actions.Count)
            {
                state.Actions.ReorderItem(result.Index, newIndex);
            }

            Refresh();
        }
    }
}