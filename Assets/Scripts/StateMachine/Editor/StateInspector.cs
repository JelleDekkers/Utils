using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the inspector for the <see cref="StateMachine"/>'s selected <see cref="global::StateMachine.State"/>. 
    /// Shows all <see cref="StateAction"/>s and variables
    /// </summary>
    public class StateInspector
    {
        private enum ContextMenuCommand
        {
            Reset,
            Delete,
            EditScript
        }

        private class ContextMenuResult
        {
            public ContextMenuCommand Command { get; private set; }
            public int Index { get; private set; }

            public ContextMenuResult(ContextMenuCommand command, int index)
            {
                Command = command;
                Index = index;
            }
        }

        public State State { get; private set; }

        private StateMachine stateMachine;
        private SerializedProperty actionsList;

        private Rect rect;

        public StateInspector(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Show(State state)
        {
            State = state;
            UpdateActionsList();
        }

        public void OnInspectorGUI(Event e)
        {
            if(State == null) { return; }

            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawHeader();
            DrawActions();
            EditorGUILayout.EndVertical();

            DrawAddNewActionButton();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            string newName = EditorGUILayout.DelayedTextField(State.Title);

            if (newName != State.Title)
            {
                //Undo.RecordObject(State, "Change Name");
                State.Title = newName;
            }

            GUI.enabled = stateMachine.StartState != State;
            if (GUILayout.Button("Starter State", GUILayout.Width(90)))
            {
                stateMachine.SetStartingState(State);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActions()
        {
            EditorGUILayout.LabelField("State Actions", EditorStyles.boldLabel);
            //for (int i = 0; i < actionsList.arraySize; i++)
            //{
            //    DrawDividerLine();
            //    DrawPropertyField(i);
            //}
        }

        private void DrawAddNewActionButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add New Action", GUILayout.MaxWidth(200), GUILayout.MaxHeight(25)))
            {
                OpenActionsWindow(State);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawDividerLine()
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(1)); 
        }

        private void DrawPropertyField(int index)
        {
            SerializedProperty property = actionsList.GetArrayElementAtIndex(index);
            if (property.objectReferenceValue == null) { return; }

            GUIStyle myStyle = new GUIStyle();
            myStyle.margin = new RectOffset(15, 0, 0, 0);

            Rect header = EditorGUILayout.BeginHorizontal(myStyle);
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, GUIContent.none, true);
            EditorGUI.LabelField(header, GetPropertyName(property), EditorStyles.largeLabel);
            DrawContextMenuDropdown(index);
            EditorGUILayout.EndHorizontal();

            if (!property.isExpanded) { return; }
            EditorGUI.indentLevel++;

            SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);
            if (targetObject == null) { return; }

            SerializedProperty field = targetObject.GetIterator();
            field.NextVisible(true);

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUI.indentLevel++;

            field = targetObject.GetIterator();
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
                    Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same object iside a nested structure.");
                }
            }

            targetObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        private void UpdateActionsList()
        {
            if (State != null)
            {
                //actionsList = new SerializedObject(State).FindProperty("Actions");
            }
        }

        private void OpenActionsWindow(State state)
        {
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, "Choose Action to add");
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                OnActionSelected(t);
                window.Close();
            };
            window.RetrieveTypes<StateAction>(func);
        }

        private void OnActionSelected(Type type)
        {
            StateAction stateAction = ScriptableObject.CreateInstance(type) as StateAction;
            State.AddAction(stateAction);
            UpdateActionsList();
        }

        private string GetPropertyName(SerializedProperty property)
        {
            string s = property.objectReferenceValue.ToString();

            string[] trim = { "(", ")" };
            for (int i = 0; i < trim.Length; i++)
            {
                s = s.Replace(trim[i], "");
            }

            return s;
        }

        private void DrawContextMenuDropdown(int index)
        {
            GUIStyle iconStyle = new GUIStyle("IconButton");
            GUIContent iconContent = EditorGUIUtility.IconContent("_Popup");
            Vector2 size = iconStyle.CalcSize(iconContent);

            if (GUILayout.Button(iconContent, iconStyle, GUILayout.MaxWidth(size.x)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (ContextMenuCommand command in (ContextMenuCommand[])Enum.GetValues(typeof(ContextMenuCommand)))
                {
                    menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(command.ToString())), false, OnContextMenuButtonPressed, new ContextMenuResult(command, index));
                    //menu.AddSeparator("");
                }
                menu.ShowAsContext();
            }
        }

        private void OnContextMenuButtonPressed(object o)
        {
            ContextMenuResult result = (ContextMenuResult)o;
            StateAction stateAction = State.Actions[result.Index];
            
            switch(result.Command)
            {
                case ContextMenuCommand.EditScript:
                    OpenScript(stateAction);
                    break;
                case ContextMenuCommand.Reset:
                    throw new System.NotImplementedException();
                    //UpdateActionsList();
                    //break;
                case ContextMenuCommand.Delete:
                    State.RemoveAction(stateAction);
                    UpdateActionsList();
                    break;
            }
        }

        private void OpenScript(ScriptableObject obj)
        {
            MonoScript script = MonoScript.FromScriptableObject(obj);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }
    }
}
 