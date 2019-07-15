using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class InspectorTestRuleGroup : InspectorTest
    {
        private const string PROPERTY_NAME = "rules";

        public InspectorTestRuleGroup(StateMachineData stateMachine, ScriptableObject target, string propertyName) : base(stateMachine, target) { }

        protected override void DrawInspectorContent(Event e)
        {
            DrawHeader();
            DrawDividerLine();
            DrawPropertyFields(PROPERTY_NAME);
        }
    }
}