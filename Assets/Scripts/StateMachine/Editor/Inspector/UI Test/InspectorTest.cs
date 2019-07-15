using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class InspectorTest
    {
        protected enum ContextMenuCommand
        {
            EditScript,
            MoveUp,
            MoveDown,
            Copy,
            Paste,
            Delete,
            Reset
        }

        protected class ContextMenuResult
        {
            public ContextMenuCommand Command { get; private set; }
            public int Index { get; private set; }

            public ContextMenuResult(ContextMenuCommand command, int index)
            {
                Command = command;
                Index = index;
            }
        }
        
        protected StateMachineData StateMachine { get; private set; }
        protected ScriptableObject Target { get; private set; }

        private SerializedObject serializedObject = null;

        public InspectorTest(StateMachineData stateMachine, ScriptableObject target)
        {
            StateMachine = stateMachine;
            Target = target;
            serializedObject = new SerializedObject(Target);

            Refresh();
        }
        
        public void Refresh()
        {
            Debug.Log("remove this");
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawInspectorContent(e);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawInspectorContent(Event e)
        {
            //if (!StateMachineRenderer.debug)
            //{
            //    DrawHeader();
            //    DrawStateFields();
            //}
            //else
            //{
            //DrawProperties(SerializedObject);
            //}

            DrawHeader();
            DrawDividerLine();
            DrawAllProperties();
            DrawDividerLine();
            //DrawPropertyFields();
        }

        protected void DrawPropertyFields(string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            for (int i = 0; i < property.arraySize; i++)
            {
                DrawDividerLine();
                DrawPropertyField(property, i);
            }

            if (property.arraySize == 0)
            {
                DrawDividerLine();
                GUILayout.Label("Empty");
            }
        }

        protected void DrawPropertyField(SerializedProperty property, int index)
        {
            if (property.objectReferenceValue == null) { return; }

            Rect header = EditorGUILayout.BeginHorizontal(GUIStyles.InspectorStyle);
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, GUIContent.none, true);
            EditorGUI.LabelField(header, NicifyPropertyName(property), EditorStyles.largeLabel);
            DrawContextMenuDropdown(index);
            EditorGUILayout.EndHorizontal();

            if (!property.isExpanded) { return; }
            EditorGUI.indentLevel++;

            SerializedObject obj = new SerializedObject(property.objectReferenceValue);
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
                    Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same object iside a nested structure.");
                }
            }

            obj.ApplyModifiedProperties();

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        protected void DrawAllProperties()
        {
            SerializedObject serializedObject = new SerializedObject(Target);
            SerializedProperty property = serializedObject.GetIterator();

            if (property.NextVisible(true))
            {
                do
                {
                    if (property.isArray) { EditorGUI.indentLevel++; }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(property.name), true);
                    if (property.isArray) { EditorGUI.indentLevel--; }
                }
                while (property.NextVisible(false));
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected string NicifyPropertyName(SerializedProperty property)
        {
            string s = property.objectReferenceValue.ToString();

            string[] trim = { "(", ")" };
            for (int i = 0; i < trim.Length; i++)
            {
                s = s.Replace(trim[i], "");
            }

            return s;
        }

        protected void DrawContextMenuDropdown(int index)
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

            switch (result.Command)
            {
                case ContextMenuCommand.EditScript:
                    OnEditScriptButtonPressed(result.Index);
                    break;
                case ContextMenuCommand.Reset:
                    OnResetButtonPressed(result.Index);
                    break;
                case ContextMenuCommand.Delete:
                    OnDeleteButtonPressed(result.Index);
                    break;
            }
        }

        protected virtual void DrawDividerLine(float height = 1)
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(height));
        }

        protected virtual void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            //string newName = EditorGUILayout.DelayedTextField(State.Title);

            //if (newName != State.Title)
            //{
                Undo.RecordObject(StateMachine, "Change State Name");
                //State.Title = newName;
                EditorUtility.SetDirty(Target);
            //}

            //GUI.enabled = StateMachine.EntryState != State;
            if (GUILayout.Button("Entry State", GUILayout.Width(90)))
            {
                Undo.RecordObject(StateMachine, "Set Entry State");
                //StateMachine.SetEntryState(State);
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

        protected void OpenTypeFilterWindow()
        {
            // todo: check {0} name
            string typeName = "";
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", typeName.ToString()));
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
            string assetFilePath = AssetDatabase.GetAssetPath(Target);
            StateAction stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(type, assetFilePath) as StateAction;

            //State.AddAction(stateAction);
            Refresh();
        }

        protected void OnEditScriptButtonPressed(int index)
        {
            MonoScript script = MonoScript.FromScriptableObject(Target as ScriptableObject);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }

        protected void OnDeleteButtonPressed(int index)
        {
            Undo.RecordObject(StateMachine, "Remove Action");

            //StateAction action = State.Actions[index];
            //State.RemoveAction(action);
            //UnityEngine.Object.DestroyImmediate(action, true);

            Refresh();
        }

        protected virtual void OnResetButtonPressed(int index)
        {
            throw new NotImplementedException();
        }
    }
}
