﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class InspectorUI
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

        protected ScriptableObject InspectedObject { get; private set; }
        protected string PropertyFieldName { get; private set; }
        protected SerializedProperty InspectedProperty { get; private set; }
        protected StateMachine StateMachine { get; private set; }

        public void Show(StateMachine stateMachine, IInspectable inspectedObject)
        {
            StateMachine = stateMachine;
            InspectedObject = inspectedObject.InspectableObject;
            PropertyFieldName = inspectedObject.PropertyFieldName;

            Refresh();
        }

        public void Refresh()
        {
            InspectedProperty = new SerializedObject(InspectedObject).FindProperty(PropertyFieldName);
        }

        public void OnInspectorGUI(Event e)
        {
            if (InspectedObject == null) { return; }

            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawHeader();
            DrawProperties();
            EditorGUILayout.EndVertical();

            DrawAddNewButton();
        }

        protected virtual void DrawHeader() { }

        protected void DrawProperties()
        {
            EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(PropertyFieldName), EditorStyles.boldLabel);
            if (InspectedProperty == null) { return; }

            for (int i = 0; i < InspectedProperty.arraySize; i++)
            {
                DrawDividerLine();
                DrawPropertyField(i);
            }
        }

        protected void DrawPropertyField(int index)
        {
            SerializedProperty property = InspectedProperty.GetArrayElementAtIndex(index);
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
            string typeName = ObjectNames.NicifyVariableName(PropertyFieldName);
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", typeName));
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                CreateNewType(t);
                window.Close();
            };
            window.RetrieveTypes<StateAction>(func);
        }

        protected virtual void CreateNewType(Type type) { }

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
            OpenScript(InspectedObject);
        }

        protected virtual void OnResetButtonPressed(int index)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnDeleteButtonPressed(int index)
        {
            Refresh();
        }

        private void OpenScript(ScriptableObject obj)
        {
            MonoScript script = MonoScript.FromScriptableObject(obj);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }

        private void DrawDividerLine()
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(1));
        }
    }
}