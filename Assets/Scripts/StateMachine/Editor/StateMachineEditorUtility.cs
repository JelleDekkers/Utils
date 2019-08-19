using System;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object; 

namespace Utils.Core.Flow
{
    public static class StateMachineEditorUtility
    {
        public static Action<IStateMachineData> StateMachineClearedEvent;
        public static Action<IStateMachineData, State> StateAddedEvent;
        public static Action<IStateMachineData, State> StateRemovedEvent;
        public static Action<ScriptableObject> ObjectResetEvent;
        public static Action<State, RuleGroup> RuleGroupAddedEvent;
        public static Action<State, RuleGroup> RuleGroupRemovedEvent;

        public static void ClearStateMachine(this IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Clear Machine");

            for (int i = 0; i < data.States.Count; i++)
            {
                State state = data.States[i];
                foreach (StateAction action in state.TemplateActions)
                {
                    Object.DestroyImmediate(action, true);
                }

                foreach (RuleGroup ruleGroup in state.RuleGroups)
                {
                    foreach (Rule rule in ruleGroup.TemplateRules)
                    {
                        Object.DestroyImmediate(rule, true);
                    }
                    
                }
                Object.DestroyImmediate(state, true);
            }

            data.States.Clear();
            StateMachineClearedEvent?.Invoke(data);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static State CreateNewState(this IStateMachineData data, Vector2 position)
        {
            Undo.RecordObject(data.SerializedObject, "Add State");

            State state;
            if (data.SerializedObject is ScriptableObject)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(data.SerializedObject);
                state = StateMachineEditorUtilityHelper.CreateObjectInstance<State>(assetFilePath);
            }
            else
            {
                state = ScriptableObject.CreateInstance<State>();
            }

            state.Position = position;
            data.AddNewState(state);

            StateAddedEvent?.Invoke(data, state);
            EditorUtility.SetDirty(data.SerializedObject);

            return state;
        }

        public static void RemoveState(this IStateMachineData data, State state)
        {
            StateRemovedEvent?.Invoke(data, state);

            Undo.RecordObject(data.SerializedObject, "Remove State");

            bool isEntryState = data.EntryState == state;

            data.States.Remove(state);
            Object.DestroyImmediate(state, true);

            if (isEntryState && data.States.Count > 0)
            {
                SetEntryState(data, data.States[0]);
            }

            foreach(StateAction action in state.TemplateActions)
            {
                Object.DestroyImmediate(action, true);
            }

            foreach (RuleGroup ruleGroup in state.RuleGroups)
            {
                foreach(Rule rule in ruleGroup.TemplateRules)
                {
                    Object.DestroyImmediate(rule, true);
                }
            }

            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void SetEntryState(this IStateMachineData data, State state)
        {
            Undo.RecordObject(data.SerializedObject, "Set Entry State");
            data.EntryState = state;
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void ClearActions(this State state)
        {
            Undo.RecordObject(state, "Clear State Actions");

            for (int i = 0; i < state.TemplateActions.Count; i++)
            {
                Object.DestroyImmediate(state.TemplateActions[i], true);
            }

            state.TemplateActions.Clear();

            EditorUtility.SetDirty(state);
        }

        public static void ClearRules(this State state)
        {
            Undo.RecordObject(state, "Clear State Rules");

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                foreach (Rule rule in state.RuleGroups[i].TemplateRules)
                {
                    Object.DestroyImmediate(rule, true);
                }
            }

            state.RuleGroups.Clear();

            EditorUtility.SetDirty(state);
        }

        public static void AddStateAction(this State state, Type actionType, IStateMachineData data)
        {
            Undo.RecordObject(state, "Add Action");

            StateAction stateAction = null;
            if (data is ScriptableObject)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(state);
                stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(actionType, assetFilePath) as StateAction;
            }
            else
            {
                stateAction = ScriptableObject.CreateInstance(actionType) as StateAction;
            }

            state.TemplateActions.Add(stateAction);

            EditorUtility.SetDirty(state);
        }

        public static void RemoveStateAction(this State state, StateAction stateAction)
        {
            Undo.RecordObject(state, "Remove Action");

            state.TemplateActions.Remove(stateAction);
            Object.DestroyImmediate(stateAction, true);
            EditorUtility.SetDirty(state);
        }

        public static void Reset(this StateAction action, State state)
        {
            Undo.RecordObject(state, "Reset Action");

            string assetFilePath = AssetDatabase.GetAssetPath(action);
            StateAction newAction = StateMachineEditorUtilityHelper.CreateObjectInstance(action.GetType(), assetFilePath) as StateAction;
            int index = state.TemplateActions.IndexOf(action);
            Object.DestroyImmediate(action, true);
            state.TemplateActions[index] = newAction;

            ObjectResetEvent?.Invoke(state);
            EditorUtility.SetDirty(state);
        }

        public static RuleGroup AddNewRuleGroup(this State state)
        {
            Undo.RecordObject(state, "Add RuleGroup");

            RuleGroup group = new RuleGroup();
            state.RuleGroups.Add(group);

            RuleGroupAddedEvent?.Invoke(state, group);
            EditorUtility.SetDirty(state);

            return group;
        }

        public static void RemoveRuleGroup(this State state, RuleGroup group)
        {
            foreach(Rule rule in group.TemplateRules)
            {
                Object.DestroyImmediate(rule, true);
            }

            state.RuleGroups.Remove(group);
            RuleGroupRemovedEvent?.Invoke(state, group);

            EditorUtility.SetDirty(state);
        }

        public static void Clear(this RuleGroup ruleGroup, State state)
        {
            Undo.RecordObject(state, "Clear RuleGroup");

            foreach (Rule rule in ruleGroup.TemplateRules)
            {
                Object.DestroyImmediate(rule, true);
            }
            ruleGroup.TemplateRules.Clear();

            EditorUtility.SetDirty(state);
        }

        public static void Reset(this Rule rule, RuleGroup ruleGroup, IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Reset Rule");

            Rule newRule;
            if (data is ScriptableObject)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(rule);
                newRule = StateMachineEditorUtilityHelper.CreateObjectInstance(rule.GetType(), assetFilePath) as Rule;
            }
            else
            {
                newRule = ScriptableObject.CreateInstance(rule.GetType()) as Rule;
            }

            Object.DestroyImmediate(rule, true);
            int index = ruleGroup.TemplateRules.IndexOf(rule);
            ruleGroup.TemplateRules[index] = newRule;

            ObjectResetEvent?.Invoke(rule);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void AddNewRule(this RuleGroup group, IStateMachineData data, Type ruleType)
        {
            // TODO: needs to be state, not the statemachine?
            Undo.RecordObject(data.SerializedObject, "Add Rule");

            Rule rule;
            if (data.SerializedObject is ScriptableObject)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(data.SerializedObject);
                rule = StateMachineEditorUtilityHelper.CreateObjectInstance(ruleType, assetFilePath) as Rule;
            }
            else
            {
                rule = ScriptableObject.CreateInstance(ruleType) as Rule;
            }
            group.TemplateRules.Add(rule);

            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void RemoveRule(this RuleGroup group, Rule rule, IStateMachineData data)
        {
            // TODO: needs to be state, not the statemachine?
            Undo.RecordObject(data.SerializedObject, "Remove rule");
            group.TemplateRules.Remove(rule);
            Object.DestroyImmediate(rule, true);
            EditorUtility.SetDirty(data.SerializedObject);
        }
    }
}