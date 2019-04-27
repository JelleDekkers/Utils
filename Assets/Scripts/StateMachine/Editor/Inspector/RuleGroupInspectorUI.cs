using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the inspector for <see cref="global::StateMachine.RuleGroup"/>s
    /// Shows all <see cref="Rule"/>s and allows for modification
    /// </summary>
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspectorUI : InspectorUIBehaviour
    {
        private RuleGroup RuleGroup { get { return InspectedObject as RuleGroup; } }

        private const string PROPERTY_FIELD_NAME = "rules";

        public override void InspectorContent(Event e)
        {
            if (!StateMachineRenderer.ShowDebug)
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
                DrawDividerLine();
                GUILayout.Label("Empty");
            }
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rules", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddNewButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add New Rule", GUILayout.MaxWidth(200), GUILayout.MaxHeight(25)))
            {
                OpenTypeFilterWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void OpenTypeFilterWindow()
        {
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", typeof(Rule).ToString()));
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                CreateNewType(t);
                window.Close();
            };
            window.RetrieveTypes<Rule>(func);
        }

        protected void CreateNewType(Type type)
        {
            Undo.RecordObject(StateMachine, "Add Rule");
            string assetFilePath = AssetDatabase.GetAssetPath(InspectedObject);
            Rule rule = StateMachineEditorUtility.CreateObjectInstance(type, assetFilePath) as Rule;

            RuleGroup.AddRule(rule);
            Refresh();
        }

        protected override void OnDeleteButtonPressed(int index)
        {
            Undo.RecordObject(StateMachine, "Remove Rule");

            Rule rule = RuleGroup.Rules[index];
            RuleGroup.RemoveRule(rule);
            UnityEngine.Object.DestroyImmediate(rule, true);

            Refresh();
        }
    }
}