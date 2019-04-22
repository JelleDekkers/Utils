using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the inspector for the <see cref="global::StateMachine.State"/>s
    /// Shows all <see cref="StateAction"/>s and variables
    /// </summary>
    public class StateInspectorUI : InspectorUI
    {
        private State State { get { return InspectedProperty.serializedObject.targetObject as State; } }

        protected override void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.DelayedTextField(State.Title);

            if (newName != State.Title)
            {
                Undo.RecordObject(StateMachine, "Change State Name");
                State.Title = newName;
                EditorUtility.SetDirty(InspectedObject);
            }

            GUI.enabled = StateMachine.EntryState != State;
            if (GUILayout.Button("Entry State", GUILayout.Width(90)))
            {
                Undo.RecordObject(StateMachine, "Set Entry State");
                StateMachine.SetEntryState(State);
                EditorUtility.SetDirty(StateMachine);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        // TODO: naar base class?
        protected override void CreateNewType(Type type)
        {
            Undo.RecordObject(StateMachine, "Add Action");
            string assetFilePath = AssetDatabase.GetAssetPath(InspectedObject);
            StateAction stateAction = StateMachineEditorUtility.CreateObjectInstance(type, assetFilePath) as StateAction;

            State.AddAction(stateAction);
            Refresh();
        }

        protected override void OnDeleteButtonPressed(int index)
        {
            StateAction action = State.Actions[index];

            State.RemoveAction(action);
            UnityEngine.Object.DestroyImmediate(action, true);

            base.OnDeleteButtonPressed(index);
        }
    }
}
 