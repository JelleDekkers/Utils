﻿using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow.Inspector
{
    [CustomInspectorUI(typeof(RuleGroup))]
    public class RuleGroupInspector : IInspectorUIBehaviour
    {
        private const string PROPERTY_NAME = "TemplateRules";

        private StateMachineEditorManager manager;
        private SerializedObject serializedStateObject;
        private SerializedProperty ruleGroupProperty;
        private State state;
        private RuleGroup ruleGroup;

        public RuleGroupInspector(StateMachineEditorManager manager, State state, RuleGroup ruleGroup)
        {
            this.manager = manager;
            this.state = state;
            this.ruleGroup = ruleGroup;

            Init();
        }

        private void Init()
        {
            serializedStateObject = new SerializedObject(state);

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                if (state.RuleGroups[i] == ruleGroup)
                {
                    SerializedProperty correctRuleGroup = serializedStateObject.FindProperty("RuleGroups").GetArrayElementAtIndex(i);
                    ruleGroupProperty = correctRuleGroup.FindPropertyRelative(PROPERTY_NAME);
                    break;
                }
            }
        }

        public void Refresh()
        {
            Init();
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            InspectorUIUtility.DrawHeader("Rules", () => InspectorUIUtility.DrawAddNewButton(OnAddNewButtonPressedEvent));
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawPropertyFields(ruleGroupProperty, OnContextMenuButtonPressed);
            EditorGUILayout.EndVertical();
        }

        protected void OnAddNewButtonPressedEvent()
        {
            InspectorUIUtility.OpenTypeFilterWindow(typeof(Rule), CreateNewType);
        }

        private void CreateNewType(Type type)
        {
            ruleGroup.AddNewRule(manager.StateMachineData, type);
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
            ruleGroup.RemoveRule(ruleGroup.TemplateRules[result.Index]);
            Refresh();
        }

        private void OnResetButtonPressed(ContextMenu.Result result)
        {
            ruleGroup.TemplateRules[result.Index].Reset(ruleGroup);
            Refresh();
        }

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