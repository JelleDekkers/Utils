using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the inspector for the <see cref="global::StateMachine.State"/>s
    /// Shows all <see cref="StateAction"/>s and variables
    /// </summary>
    [CustomInspectorUI(typeof(State))]
    public class StateInspectorUI : InspectorUIBehaviour
    {
        private State State { get { return InspectedObject as State; } }

        private const string PROPERTY_FIELD_NAME = "actions";

        public override void InspectorContent(Event e)
        {
            if (!StateMachineRenderer.debug)
            {
                DrawHeader();
                DrawStateFields();
            }
            else
            {
                DrawProperties();
            }
        }

        public override void OnInspectorGUI(Event e)
        {
            base.OnInspectorGUI(e);
            DrawAddNewButton();
        }

        private void DrawStateFields()
        {
            SerializedProperty property = SerializedObject.FindProperty(PROPERTY_FIELD_NAME);

            for (int i = 0; i < property.arraySize; i++)
            {
                DrawDividerLine();
                DrawExpandedPropertyFieldArray(property, i);
            }

            if (property.arraySize == 0)
            {
                GUILayout.Label("Empty");
            }
        }

        private void DrawHeader()
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("State Actions", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddNewButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add New Action", GUILayout.MaxWidth(200), GUILayout.MaxHeight(25)))
            {
                OpenTypeFilterWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void OpenTypeFilterWindow()
        {
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", State.ToString()));
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                CreateNewType(t);
                window.Close();
            };
            window.RetrieveTypes<StateAction>(func);
        }

        protected void CreateNewType(Type type)
        {
            Undo.RecordObject(StateMachine, "Add Action");
            string assetFilePath = AssetDatabase.GetAssetPath(InspectedObject);
            StateAction stateAction = StateMachineEditorUtility.CreateObjectInstance(type, assetFilePath) as StateAction;

            State.AddAction(stateAction);
            Refresh();
        }

        protected override void OnEditScriptButtonPressed(int index)
        {
            OpenScript(State.Actions[index]);
        }

        protected override void OnDeleteButtonPressed(int index)
        {
            Undo.RecordObject(StateMachine, "Remove Action");

            StateAction action = State.Actions[index];
            State.RemoveAction(action);
            UnityEngine.Object.DestroyImmediate(action, true);

            Refresh();
        }
    }
}
 