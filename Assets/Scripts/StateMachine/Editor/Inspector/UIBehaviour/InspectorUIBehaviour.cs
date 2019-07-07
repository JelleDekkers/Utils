using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Base class for <see cref="StateMachineInspector"/> UI behaviour
    /// To create a custom behaviour, inherit from this class and use <see cref="CustomInspectorUIAttribute"/>
    /// </summary>
    public class InspectorUIBehaviour
    {
        protected enum ContextMenuCommand
        {
            Reset,
            Delete,
            EditScript
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

        protected StateMachineEditorManager StateMachineRenderer { get; private set; }
        protected StateMachineData StateMachine { get { return StateMachineRenderer.StateMachineData; } }
        protected ScriptableObject InspectedObject { get; private set; }
        protected SerializedObject SerializedObject { get; set; }

        public void Show(StateMachineEditorManager stateMachineRenderer, ScriptableObject inspectedObject)
        {
            StateMachineRenderer = stateMachineRenderer;
            InspectedObject = inspectedObject;

            Refresh();
        }

        public void Refresh()
        {
            SerializedObject = new SerializedObject(InspectedObject);
        }

        public virtual void OnInspectorGUI(Event e)
        {
            if (InspectedObject == null) { return; }

            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawInspectorContent(e);
            EditorGUILayout.EndVertical();
        }

        public virtual void DrawInspectorContent(Event e)
        {
            DrawProperties();
        }

        protected virtual void DrawProperties()
        {
            DrawAllFields(SerializedObject);
        }

        protected void DrawExpandedPropertyFieldArray(SerializedProperty property, int index)
        {
            property = property.GetArrayElementAtIndex(index);
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

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        private void DrawAllFields(SerializedObject serializedObject)
        {
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

        protected string GetPropertyName(SerializedProperty property)
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

        protected virtual void OnContextMenuButtonPressed(object o)
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

        protected virtual void OnEditScriptButtonPressed(int index)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnResetButtonPressed(int index)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnDeleteButtonPressed(int index)
        {
            throw new NotImplementedException();
        }

        protected void OpenScript(ScriptableObject obj)
        {
            MonoScript script = MonoScript.FromScriptableObject(obj);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }

        protected void DrawDividerLine(float height = 1)
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(height));
        }
    }
}