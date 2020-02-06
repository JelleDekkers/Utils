using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspector : IInspectorUIBehaviour
    {
        private const string RULES_PROPERTY_NAME = "TemplateRules"; // TODO: change this during playmode?
        private const string STATES_PROPERTY_NAME = "states";
        private const string RULEGROUPS_PROPERTY_NAME = "RuleGroups";

        private StateMachineLayerRenderer editorUI;
        private SerializedObject serializedStateMachine;
        private SerializedProperty ruleGroupProperty;
        private State state;
        private RuleGroup ruleGroup;

        private SerializedProperty rulesProperty;
        private int ruleGroupIndex;

        public RuleGroupInspector(StateMachineLayerRenderer editorUI, State state, RuleGroup ruleGroup)
        {
            this.editorUI = editorUI;
            this.state = state;
            this.ruleGroup = ruleGroup;

            Refresh();
        }

        public void Refresh()
        {
            serializedStateMachine = new SerializedObject(editorUI.StateMachineData.SerializedObject);

            SerializedProperty statesProperty = serializedStateMachine.FindProperty(STATES_PROPERTY_NAME);
            int stateIndex = editorUI.StateMachineData.States.IndexOf(state);
            SerializedProperty correctStateProperty = statesProperty.GetArrayElementAtIndex(stateIndex);
            SerializedProperty ruleGroupsProperty = correctStateProperty.FindPropertyRelative(RULEGROUPS_PROPERTY_NAME);

            for (int i = 0; i < ruleGroupsProperty.arraySize; i++)
            {
                if (state.RuleGroups[i] == ruleGroup)
                {
                    ruleGroupIndex = i;
                    rulesProperty = ruleGroupsProperty.GetArrayElementAtIndex(ruleGroupIndex).FindPropertyRelative(RULES_PROPERTY_NAME);
                }
            }
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            InspectorUIUtility.DrawHeader("Rules", () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent, "Add new Rule"));
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawArrayPropertyField(rulesProperty, ruleGroupIndex, OnContextMenuButtonPressed);
            EditorGUILayout.EndVertical();
        }

        protected void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(Rule), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            ruleGroup.AddNewRule(type, editorUI.StateMachineData);
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
                //case ContextMenu.Command.Reset:
                //    OnResetButtonPressed(result);
                //    break;
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
            ruleGroup.RemoveRule(ruleGroup.TemplateRules[result.Index], editorUI.StateMachineData); 
            Refresh();
        }

        //private void OnResetButtonPressed(ContextMenu.Result result)
        //{
        //    ruleGroup.TemplateRules[result.Index].Reset(ruleGroup, editorUI.StateMachineData);
        //    Refresh();
        //}

        private void OnReorderButtonPressed(ContextMenu.Result result, ContextMenu.ReorderDirection direction)
        {
            int newIndex = result.Index + (int)direction;
            if (newIndex >= 0 && newIndex < ruleGroup.TemplateRules.Count)
            {
                ruleGroup.TemplateRules.ReorderItem(result.Index, newIndex);
            }

            Refresh();
        }
    }
}