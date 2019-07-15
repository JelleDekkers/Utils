using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomInspectorUI(typeof(State))]
    public class StateInspector : InspectorUIBehaviour
    {
        public State State { get { return TargetObject as State; } }

        private const string PROPERTY_NAME = "actions";

        public StateInspector(StateMachineEditorManager manager, ScriptableObject target) : base(manager, target) { }

        protected override void DrawInspectorContent(Event e)
        {
            DrawHeader("State Actions", () => DrawAddNewButton(OnAddNewButtonPressedEvent));
            DrawDividerLine();
            DrawPropertyFields(PROPERTY_NAME);
        }

        protected override void DrawHeader(string title, params Action[] extraContent)
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

            base.DrawHeader(title, extraContent);
        }

        private void OnSetEntryStateButtonPressedEvent()
        {
            Manager.StateMachineData.SetEntryState(State);
        }

        protected void OnAddNewButtonPressedEvent()
        {
            OpenTypeFilterWindow(typeof(StateAction), CreateNewType);
        }

        protected void CreateNewType(Type type)
        {
            State.AddStateAction(type);
            Refresh();
        }

        protected override void OnDeleteButtonPressed(ContextMenuResult result)
        {
            State.RemoveStateAction(State.Actions[result.Index]);
            Refresh();
        }
    }
}