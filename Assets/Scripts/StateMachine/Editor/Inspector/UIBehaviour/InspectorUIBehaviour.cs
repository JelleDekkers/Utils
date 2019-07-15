﻿using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class InspectorUIBehaviour
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
            public ScriptableObject Obj { get; private set; }

            public ContextMenuResult(ScriptableObject obj, ContextMenuCommand command, int index)
            {
                Command = command;
                Index = index;
                Obj = obj;
            }
        }

        protected StateMachineEditorManager Manager { get; private set; }
        protected ScriptableObject TargetObject { get; private set; }

        private SerializedObject serializedObject = null;

        public InspectorUIBehaviour(StateMachineEditorManager manager, ScriptableObject target)
        {
            Manager = manager;
            TargetObject = target;

            Refresh();
        }

        public void Refresh()
        {
            serializedObject = new SerializedObject(TargetObject);
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawInspectorContent(e);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawInspectorContent(Event e)
        {
            DrawHeader(TargetObject.ToString());
            DrawDividerLine();
            DrawAllProperties();
        }

        protected void DrawPropertyFields(string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            for (int i = 0; i < property.arraySize; i++)
            {
                if (i != 0) { DrawDividerLine(); }
                DrawPropertyArrayField(property, i);
            }

            if (property.arraySize == 0)
            {
                GUILayout.Label("Empty");
            }
        }

        /// <summary>
        /// Draws all properties inside <see cref="property"/> from an array
        /// </summary>
        /// <param name="property"></param>
        /// <param name="index"></param>
        protected void DrawPropertyArrayField(SerializedProperty property, int index)
        {
            property = property.GetArrayElementAtIndex(index);
            if (property.objectReferenceValue == null) { return; }

            Rect header = EditorGUILayout.BeginHorizontal(GUIStyles.InspectorStyle);
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, GUIContent.none, true);
            EditorGUI.LabelField(header, NicifyPropertyName(property), EditorStyles.largeLabel);
            DrawContextMenuDropdown(index, property.objectReferenceValue as ScriptableObject);
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

        /// <summary>
        /// Draws all properties of <see cref="TargetObject"/>.
        /// </summary>
        protected void DrawAllProperties()
        {
            SerializedObject serializedObject = new SerializedObject(TargetObject);
            SerializedProperty property = serializedObject.GetIterator();

            if (property.NextVisible(true))
            {
                do
                {
                    Rect header = EditorGUILayout.BeginHorizontal(GUIStyles.InspectorStyle);
                    EditorGUIUtility.labelWidth = header.width / 2.5f;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(property.name), true);
                    EditorGUILayout.EndHorizontal();
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

        protected void DrawContextMenuDropdown(int index, ScriptableObject obj)
        {
            GUIStyle iconStyle = new GUIStyle("IconButton");
            GUIContent iconContent = EditorGUIUtility.IconContent("_Popup");
            Vector2 size = iconStyle.CalcSize(iconContent);

            if (GUILayout.Button(iconContent, iconStyle, GUILayout.MaxWidth(size.x)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (ContextMenuCommand command in (ContextMenuCommand[])Enum.GetValues(typeof(ContextMenuCommand)))
                {
                    menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(command.ToString())), false, OnContextMenuButtonPressed, new ContextMenuResult(obj, command, index));
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
                    OnEditScriptButtonPressed(result);
                    break;
                case ContextMenuCommand.Reset:
                    OnResetButtonPressed(result.Index);
                    break;
                case ContextMenuCommand.Delete:
                    OnDeleteButtonPressed(result);
                    break;
            }
        }

        protected virtual void DrawDividerLine(float height = 1)
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(height));
        }

        protected virtual void DrawHeader(string title, params Action[] extraContent)
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 13;

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < extraContent.Length; i++)
            {
                extraContent[i].Invoke();
            }
            EditorGUILayout.LabelField(title, style);
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawAddNewButton(Action buttonPressedEvent)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = GUI.skin.button.normal.background;
            style.padding.left = 1;
            style.margin.top = 3;

            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), style, GUILayout.MaxWidth(18)))
            {
                buttonPressedEvent.Invoke();
            }
        }

        protected void OpenTypeFilterWindow(Type type, Action<Type> typeSelectedEvent)
        {
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", type.Name.ToString()));
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                typeSelectedEvent.Invoke(t);
                window.Close();
            };
            window.RetrieveTypes(type, func);
        }

        protected void OnEditScriptButtonPressed(ContextMenuResult result)
        {
            OpenScript(result.Obj);
        }

        protected void OpenScript(ScriptableObject obj)
        {
            MonoScript script = MonoScript.FromScriptableObject(obj);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }

        protected virtual void OnDeleteButtonPressed(ContextMenuResult result) { }

        protected virtual void OnResetButtonPressed(int index) { }
    }
}
