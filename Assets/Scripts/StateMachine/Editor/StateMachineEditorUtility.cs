using System;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object; 

namespace StateMachine
{
    public static class StateMachineEditorUtility
    {
        public static Action<State> StateAddedEvent;
        public static Action<State> StateRemovedEvent;
        public static Action<RuleGroup> RuleGroupAddedEvent;
        public static Action<RuleGroup> RuleGroupRemovedEvent;

        public static void ClearStateMachine(this StateMachineData stateMachine)
        {
            Undo.RecordObject(stateMachine, "Clear Machine");

            for (int i = 0; i < stateMachine.States.Count; i++)
            {
                Object.DestroyImmediate(stateMachine.States[i]);
            }

            stateMachine.States.Clear();
        }

        public static State CreateNewState(this StateMachineData stateMachine)
        {
            return CreateNewState(stateMachine, Vector2.zero);
        }

        public static State CreateNewState(this StateMachineData stateMachine, Vector2 position)
        {
            Undo.RecordObject(stateMachine, "Add State");

            string assetFilePath = AssetDatabase.GetAssetPath(stateMachine);
            State state = StateMachineEditorUtilityHelper.CreateObjectInstance<State>(assetFilePath);
            state.position = position;
            stateMachine.AddNewState(state);

            StateAddedEvent?.Invoke(state);
            return state;
        }

        public static void RemoveState(this StateMachineData stateMachine, State state)
        {
            StateRemovedEvent?.Invoke(state);

            Undo.RecordObject(stateMachine, "Remove State");

            bool isEntryState = stateMachine.EntryState == state;

            stateMachine.States.Remove(state);
            Object.DestroyImmediate(state, true);

            if (isEntryState && stateMachine.States.Count > 0)
            {
                SetEntryState(stateMachine, stateMachine.States[0]);
            }

            EditorUtility.SetDirty(stateMachine);
            AssetDatabase.SaveAssets();
        }

        public static void SetEntryState(this StateMachineData stateMachine, State state)
        {
            Undo.RecordObject(stateMachine, "Set Entry State");
            stateMachine.EntryState = state;
            EditorUtility.SetDirty(stateMachine);
        }

        public static void Reset(this State state)
        {
            Undo.RecordObject(state, "Reset Action");

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                Object.DestroyImmediate(state.RuleGroups[i], true);
            }

            for (int i = 0; i < state.Actions.Count; i++)
            {
                Object.DestroyImmediate(state.Actions[i], true);
            }

            state.Actions.Clear();
            state.RuleGroups.Clear();

            EditorUtility.SetDirty(state);
            AssetDatabase.SaveAssets();
        }

        public static void AddStateAction(this State state, System.Type type)
        {
            Undo.RecordObject(state, "Add Action");

            string assetFilePath = AssetDatabase.GetAssetPath(state);
            StateAction stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(type, assetFilePath) as StateAction;
            state.Actions.Add(stateAction);
        }

        public static void RemoveStateAction(this State state, StateAction stateAction)
        {
            Undo.RecordObject(state, "Remove Action");

            state.Actions.Remove(stateAction);
            Object.DestroyImmediate(stateAction, true);
        }

        public static RuleGroup AddNewRuleGroup (this State state)
        {
            Undo.RecordObject(state, "Add RuleGroup");

            string assetFilePath = AssetDatabase.GetAssetPath(state);
            RuleGroup group = StateMachineEditorUtilityHelper.CreateObjectInstance<RuleGroup>(assetFilePath);
            state.RuleGroups.Add(group);

            RuleGroupAddedEvent?.Invoke(group);

            return group;
        }

        public static void RemoveRuleGroup(this State state, RuleGroup group)
        {
            RuleGroupRemovedEvent?.Invoke(group);

            state.RuleGroups.Remove(group);
            Object.DestroyImmediate(group, true);
        }

        public static void Reset(this Rule rule)
        {
            Undo.RecordObject(rule, "Reset rule");

            throw new System.NotImplementedException();

            EditorUtility.SetDirty(rule);
            AssetDatabase.SaveAssets();
        }

        public static void AddNewRule(this RuleGroup group, System.Type ruleType)
        {
            Undo.RecordObject(group, "Add Rule");
            string assetFilePath = AssetDatabase.GetAssetPath(group);
            Rule rule = StateMachineEditorUtilityHelper.CreateObjectInstance(ruleType, assetFilePath) as Rule;
            group.Rules.Add(rule);
        }

        public static void RemoveRule(this RuleGroup group, Rule rule)
        {
            Undo.RecordObject(group, "Remove rule");
            group.Rules.Remove(rule);
            Object.DestroyImmediate(rule);
        }
    }
}