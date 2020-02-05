using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspector : IInspectorUIBehaviour
    {
        private const string RULES_PROPERTY_NAME = "Rules";

        private StateMachineLayerRenderer editorUI;
        private SerializedObject serializedStateMachine;
        private SerializedProperty ruleGroupProperty;
        private State state;
        private RuleGroup ruleGroup;

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

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                if (state.RuleGroups[i] == ruleGroup)
                {
                    // TODO: fix this
                    //SerializedProperty correctRuleGroup = serializedStateObject.FindProperty("RuleGroups").GetArrayElementAtIndex(i);
                    //ruleGroupProperty = correctRuleGroup.FindPropertyRelative(PROPERTY_NAME);
                    break;
                }
            }
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            InspectorUIUtility.DrawHeader("Rules", () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent));
            InspectorUIUtility.DrawHorizontalLine();
            //InspectorUIUtility.DrawPropertyFields(ruleGroupProperty, OnContextMenuButtonPressed);


            SerializedProperty statesProperty = serializedStateMachine.FindProperty("states");
            int stateIndex = editorUI.StateMachineData.States.IndexOf(state);
            SerializedProperty correctStateProperty = statesProperty.GetArrayElementAtIndex(stateIndex);
            SerializedProperty ruleGroupsProperty = correctStateProperty.FindPropertyRelative("RuleGroups");
            
            for (int i = 0; i < ruleGroupsProperty.arraySize; i++)
            {
                if (state.RuleGroups[i] == ruleGroup)
                {
                    SerializedProperty rulesProperty = ruleGroupsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Rules");
                    InspectorUIUtility.DrawPropertyArrayField(rulesProperty, i);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRuleGroupUI(int index)
        {

        }

        protected void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(Rule), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            ruleGroup.AddNewRule(type, editorUI.StateMachineData, null); // TODO: fix null
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
            ruleGroup.RemoveRule(ruleGroup.Rules[result.Index], null); // TODO: fix null
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
            if (newIndex >= 0 && newIndex < ruleGroup.Rules.Count)
            {
                ruleGroup.Rules.ReorderItem(result.Index, newIndex);
            }

            Refresh();
        }
    }
}