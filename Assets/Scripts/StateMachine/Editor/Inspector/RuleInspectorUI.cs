using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the inspector for <see cref="global::StateMachine.Rule"/>s
    /// </summary>
    [CustomInspectorUI(typeof(Rule))]
    public class RuleInspectorUI : InspectorUI
    {
        private Rule Rule { get { return InspectedObject as Rule; } }

        private const string PROPERTY_FIELD_NAME = "rules";

        public override void InspectorContent(Event e)
        {
            DrawHeader();
            //base.DrawProperties();
            DrawAllFields(new SerializedObject(InspectedObject));
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rules", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            DrawDividerLine();
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

    }
}