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
        public static Action<ScriptableObject> ObjectResetEvent;
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
            EditorUtility.SetDirty(stateMachine);
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
            EditorUtility.SetDirty(stateMachine);

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

            foreach(StateAction action in state.Actions)
            {
                Object.DestroyImmediate(action, true);
            }

            foreach (RuleGroup ruleGroup in state.RuleGroups)
            {
                foreach(Rule rule in ruleGroup.Rules)
                {
                    Object.DestroyImmediate(rule, true);
                }
                Object.DestroyImmediate(ruleGroup, true);
            }

            EditorUtility.SetDirty(stateMachine);
        }

        public static void SetEntryState(this StateMachineData stateMachine, State state)
        {
            Undo.RecordObject(stateMachine, "Set Entry State");
            stateMachine.EntryState = state;
            EditorUtility.SetDirty(stateMachine);
        }

        public static void ClearActions(this State state)
        {
            Undo.RecordObject(state, "Clear State Actions");

            for (int i = 0; i < state.Actions.Count; i++)
            {
                Object.DestroyImmediate(state.Actions[i], true);
            }

            state.Actions.Clear();

            EditorUtility.SetDirty(state);
        }

        public static void ClearRules(this State state)
        {
            Undo.RecordObject(state, "Clear State Rules");

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                Object.DestroyImmediate(state.RuleGroups[i], true);
            }

            state.RuleGroups.Clear();

            EditorUtility.SetDirty(state);
        }

        public static void AddStateAction(this State state, Type type)
        {
            Undo.RecordObject(state, "Add Action");

            string assetFilePath = AssetDatabase.GetAssetPath(state);
            StateAction stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(type, assetFilePath) as StateAction;
            state.Actions.Add(stateAction);

            EditorUtility.SetDirty(state);
        }

        public static void RemoveStateAction(this State state, StateAction stateAction)
        {
            Undo.RecordObject(state, "Remove Action");

            state.Actions.Remove(stateAction);
            Object.DestroyImmediate(stateAction, true);
            EditorUtility.SetDirty(state);
        }

        public static void Reset(this StateAction action, State state)
        {
            Undo.RecordObject(state, "Reset Action");

            string assetFilePath = AssetDatabase.GetAssetPath(action);
            StateAction newAction = StateMachineEditorUtilityHelper.CreateObjectInstance(action.GetType(), assetFilePath) as StateAction;
            int index = state.Actions.IndexOf(action);
            Object.DestroyImmediate(action, true);
            state.Actions[index] = newAction;

            ObjectResetEvent?.Invoke(state);
            EditorUtility.SetDirty(state);
        }

        public static RuleGroup AddNewRuleGroup(this State state)
        {
            Undo.RecordObject(state, "Add RuleGroup");

            string assetFilePath = AssetDatabase.GetAssetPath(state);
            RuleGroup group = StateMachineEditorUtilityHelper.CreateObjectInstance<RuleGroup>(assetFilePath);
            state.RuleGroups.Add(group);

            RuleGroupAddedEvent?.Invoke(group);
            EditorUtility.SetDirty(state);

            return group;
        }

        public static void RemoveRuleGroup(this State state, RuleGroup group)
        {
            RuleGroupRemovedEvent?.Invoke(group);

            foreach(Rule rule in group.Rules)
            {
                Object.DestroyImmediate(rule, true);
            }

            state.RuleGroups.Remove(group);
            Object.DestroyImmediate(group, true);
            EditorUtility.SetDirty(state);
        }

        public static void Clear(this RuleGroup ruleGroup)
        {
            Undo.RecordObject(ruleGroup, "Clear RuleGroup");

            foreach(Rule rule in ruleGroup.Rules)
            {
                Object.DestroyImmediate(rule, true);
            }
            ruleGroup.Rules.Clear();

            EditorUtility.SetDirty(ruleGroup);
        }

        public static void Reset(this Rule rule, RuleGroup ruleGroup)
        {
            Undo.RecordObject(ruleGroup, "Reset Action");

            string assetFilePath = AssetDatabase.GetAssetPath(rule);
            Rule newRule = StateMachineEditorUtilityHelper.CreateObjectInstance(rule.GetType(), assetFilePath) as Rule;
            int index = ruleGroup.Rules.IndexOf(rule);
            Object.DestroyImmediate(rule, true);
            ruleGroup.Rules[index] = newRule;

            ObjectResetEvent?.Invoke(ruleGroup);
            EditorUtility.SetDirty(ruleGroup);
        }

        public static void AddNewRule(this RuleGroup group, Type ruleType)
        {
            Undo.RecordObject(group, "Add Rule");
            string assetFilePath = AssetDatabase.GetAssetPath(group);
            Rule rule = StateMachineEditorUtilityHelper.CreateObjectInstance(ruleType, assetFilePath) as Rule;
            group.Rules.Add(rule);
            EditorUtility.SetDirty(group);
        }

        public static void RemoveRule(this RuleGroup group, Rule rule)
        {
            Undo.RecordObject(group, "Remove rule");
            group.Rules.Remove(rule);
            Object.DestroyImmediate(rule, true);
            EditorUtility.SetDirty(group);
        }
    }
}