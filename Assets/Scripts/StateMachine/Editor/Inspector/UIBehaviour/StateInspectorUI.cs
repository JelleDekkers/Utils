using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomInspectorUI(typeof(State))]
    public class StateInspector : InspectorUIBehaviour
    {
        private const string PROPERTY_NAME = "actions";
        private State State { get { return TargetObject as State; } }

        public StateInspector(StateMachineEditorManager manager, ScriptableObject target) : base(manager, target) { }

        protected override void DrawInspectorContent(Event e)
        {
            DrawHeader("State Actions");
            InspectorUIUtility.DrawDividerLine();
            InspectorUIUtility.DrawPropertyFields(SerializedObject, PROPERTY_NAME, OnContextMenuButtonPressed);          
        }

        protected void DrawHeader(string title)
        {
            GUIStyle buttonStyle = new GUIStyle("button");
            buttonStyle.margin.top = 0;

            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.DelayedTextField(State.Title);

            if (newName != State.Title)
            {
                Undo.RecordObject(Manager.StateMachineData, "Change State Name");
                State.Title = newName;
                EditorUtility.SetDirty(TargetObject);
            }

            GUI.enabled = Manager.StateMachineData.EntryState != State;
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
            Manager.StateMachineData.SetEntryState(State);
        }

        private void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(StateAction), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            State.AddStateAction(type);
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
                case ContextMenu.Command.Reset:
                    OnResetButtonPressed(result);
                    break;
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
            State.RemoveStateAction(State.Actions[result.Index]);
            Refresh();
        }

        private void OnResetButtonPressed(ContextMenu.Result result)
        {
            State.Actions[result.Index].Reset(State);
            Refresh();
        }

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < State.Actions.Count)
            {
                State.Actions.ReorderItem(result.Index, newIndex);
            }

            Manager.Inspector.Refresh();
        }
    }
}