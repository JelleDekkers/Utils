using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspector : InspectorUIBehaviour
    {
        public RuleGroup RuleGroup { get { return TargetObject as RuleGroup; } }

        private const string PROPERTY_NAME = "rules";

        public RuleGroupInspector(StateMachineEditorManager manager, ScriptableObject target) : base(manager, target) { }

        protected override void DrawInspectorContent(Event e)
        {
            DrawHeader("Rules", () => DrawAddNewButton(OnAddNewButtonPressedEvent));
            DrawDividerLine();
            DrawPropertyFields(PROPERTY_NAME);
        }

        protected void OnAddNewButtonPressedEvent()
        {
            OpenTypeFilterWindow(typeof(Rule), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            RuleGroup.AddNewRule(type);
            Refresh();
        }

        protected override void OnDeleteButtonPressed(ContextMenuResult result)
        {
            RuleGroup.RemoveRule(RuleGroup.Rules[result.Index]);
            Refresh();
        }

        protected override void OnResetButtonPressed(int index)
        {
            RuleGroup.Rules[index].Reset();
        }
    }
}