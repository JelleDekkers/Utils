using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspector : InspectorUIBehaviour
    {
        private const string PROPERTY_NAME = "rules";
        private RuleGroup RuleGroup { get { return TargetObject as RuleGroup; } }

        public RuleGroupInspector(StateMachineEditorManager manager, ScriptableObject target) : base(manager, target) { }

        protected override void DrawInspectorContent(Event e)
        {
            InspectorUIUtility.DrawHeader("Rules", () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent));
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawPropertyFields(SerializedObject, PROPERTY_NAME, OnContextMenuButtonPressed);
        }

        protected void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(Rule), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            RuleGroup.AddNewRule(type);
            Refresh();
        }


        private void OnContextMenuButtonPressed(object o)
        {
            ContextMenu.Result result = (ContextMenu.Result)o;

            switch (result.Command)
            {
                case ContextMenu.Command.EditScript:
                    OnEditScriptButtonPressed(result);
                    break;
                case ContextMenu.Command.MoveUp:
                    OnReorderButtonPressed(result, ContextMenu.ReorderDirection.Up);
                    break;
                case ContextMenu.Command.MoveDown:
                    OnReorderButtonPressed(result, ContextMenu.ReorderDirection.Down);
                    break;
                case ContextMenu.Command.Reset:
                    OnResetButtonPressed(result);
                    break;
                case ContextMenu.Command.Delete:
                    OnDeleteButtonPressed(result);
                    break;
            }
        }

        private void OnEditScriptButtonPressed(ContextMenu.Result result)
        {
            InspectorUIUtility.OpenScript(result.Obj);
        }

        private void OnDeleteButtonPressed(ContextMenu.Result result)
        {
            RuleGroup.RemoveRule(RuleGroup.Rules[result.Index]);
            Refresh();
        }

        private void OnResetButtonPressed(ContextMenu.Result result)
        {
            RuleGroup.Rules[result.Index].Reset(RuleGroup);
            Refresh();
        }

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < RuleGroup.Rules.Count)
            {
                RuleGroup.Rules.ReorderItem(result.Index, newIndex);
            }

            Manager.Inspector.Refresh();
        }
    }
}