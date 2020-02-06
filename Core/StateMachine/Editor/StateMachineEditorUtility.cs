using System;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object; 

namespace Utils.Core.Flow
{
    public static class StateMachineEditorUtility
    {
        public const string UNDO_INSPECTOR_COMMAND_NAME = "Inspector";

        public static Action<IStateMachineData> StateMachineClearedEvent;
        public static Action<IStateMachineData, State> StateAddedEvent;
        public static Action<IStateMachineData, State> StateRemovedEvent;
        public static Action<IStateMachineData> ObjectResetEvent;
        public static Action<State, RuleGroup> RuleGroupAddedEvent;
        public static Action<State, RuleGroup> RuleGroupRemovedEvent;

        public static void ClearStateMachine(this IStateMachineData data)
        {
            Undo.RegisterCompleteObjectUndo(data.SerializedObject, "Clear StateMachine"); 

            for (int i = data.States.Count - 1; i >= 0; i--)
            {
                RemoveState(data, data.States[i], false);
            }

            data.States.Clear();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);

            StateMachineClearedEvent?.Invoke(data);
        }

        public static State CreateNewState(this IStateMachineData data, Vector2 position)
        {
            Undo.RecordObject(data.SerializedObject, "Add State");

            State state = new State();
            state.Position = position;
            data.AddState(state);

            StateAddedEvent?.Invoke(data, state);
            EditorUtility.SetDirty(data.SerializedObject);

            return state;
        }

        // TODO: doesn't work, fix:
        public static void RemoveState(this IStateMachineData data, State state, bool callEvent = true)
        {
            if (callEvent)
            {
                StateRemovedEvent?.Invoke(data, state);
            }

            Undo.RecordObject(data.SerializedObject, "Remove State");

            foreach(StateAction action in state.TemplateActions)
            {
                Undo.DestroyObjectImmediate(action);
            }

            foreach (RuleGroup ruleGroup in state.RuleGroups)
            {
                foreach(Rule rule in ruleGroup.TemplateRules)
                {
                    Undo.DestroyObjectImmediate(rule);
                }
            }

            data.RemoveState(state);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void SetEntryState(this IStateMachineData data, State state)
        {
            Undo.RecordObject(data.SerializedObject, "Set Entry State");
            data.SetEntryState(state);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void ClearActions(this State state, IStateMachineData data)
        {
            Undo.RegisterCompleteObjectUndo(data.SerializedObject, "Clear State Actions");

            for (int i = state.TemplateActions.Count - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(state.TemplateActions[i]);
            }

            state.TemplateActions.Clear();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void ClearRules(this State state, IStateMachineData data)
        {
            Undo.RegisterCompleteObjectUndo(data.SerializedObject, "Clear State Rules");

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                foreach (Rule rule in state.RuleGroups[i].TemplateRules)
                {
                    Undo.DestroyObjectImmediate(rule);
                }
            }

            state.RuleGroups.Clear();
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void AddStateAction(this State state, Type actionType, IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Add Action");

            StateAction stateAction = null;
            if (data is ScriptableObject)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(data.SerializedObject);
                stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(actionType, assetFilePath) as StateAction;
            }
            else
            {
                stateAction = ScriptableObject.CreateInstance(actionType) as StateAction;
            }

            state.TemplateActions.Add(stateAction);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void RemoveStateAction(this State state, StateAction stateAction, IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Remove Action");

            state.TemplateActions.Remove(stateAction);
            Undo.DestroyObjectImmediate(stateAction);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        //public static void Reset(this StateAction action, State state, IStateMachineData data)
        //{
        //    Undo.RecordObject(state, "Reset Action");

        //    StateAction newAction;
        //    if (data is ScriptableObject)
        //    {
        //        string assetFilePath = AssetDatabase.GetAssetPath(action);
        //        newAction = StateMachineEditorUtilityHelper.CreateObjectInstance(action.GetType(), assetFilePath) as StateAction;
        //    }
        //    else
        //    {
        //        newAction = ScriptableObject.CreateInstance(action.GetType()) as StateAction;
        //    }

        //    int index = state.TemplateActions.IndexOf(action);
        //    Undo.DestroyObjectImmediate(action);
        //    state.TemplateActions[index] = newAction;

        //    ObjectResetEvent?.Invoke(state);
        //    EditorUtility.SetDirty(state);
        //}

        public static RuleGroup AddNewRuleGroup(this State state, IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Add RuleGroup");

            RuleGroup group = new RuleGroup();
            state.RuleGroups.Add(group);

            RuleGroupAddedEvent?.Invoke(state, group);
            EditorUtility.SetDirty(data.SerializedObject);

            return group;
        }

        public static void RemoveRuleGroup(this State state, RuleGroup group, IStateMachineData data)
        {
            Undo.RecordObject(data.SerializedObject, "Remove RuleGroup");

            foreach (Rule rule in group.TemplateRules)
            {
                Object.DestroyImmediate(rule, true);
            }

            state.RuleGroups.Remove(group);
            RuleGroupRemovedEvent?.Invoke(state, group);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void Clear(this RuleGroup ruleGroup, IStateMachineData data)
        {
            Undo.RegisterCompleteObjectUndo(data.SerializedObject, "Clear RuleGroup");

            for (int i = ruleGroup.TemplateRules.Count - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(ruleGroup.TemplateRules[i]);
            }

            ruleGroup.TemplateRules.Clear();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);
        }

        //public static void Reset(this Rule rule, RuleGroup ruleGroup, IStateMachineData data)
        //{
        //    Undo.RecordObject(data.SerializedObject, "Reset Rule");

        //    Rule newRule;
        //    if (data is ScriptableObject)
        //    {
        //        string assetFilePath = AssetDatabase.GetAssetPath(rule);
        //        newRule = StateMachineEditorUtilityHelper.CreateObjectInstance(rule.GetType(), assetFilePath) as Rule;
        //    }
        //    else
        //    {
        //        newRule = ScriptableObject.CreateInstance(rule.GetType()) as Rule;
        //    }

        //    Object.DestroyImmediate(rule, true);
        //    int index = ruleGroup.TemplateRules.IndexOf(rule);
        //    ruleGroup.TemplateRules[index] = newRule;

        //    ObjectResetEvent?.Invoke(rule);
        //    EditorUtility.SetDirty(data.SerializedObject);
        //}

        public static void AddNewRule(this RuleGroup group, Type ruleType, IStateMachineData data)
        {
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
            Undo.RecordObject(data.SerializedObject, "Remove rule");
            group.TemplateRules.Remove(rule);
            Undo.DestroyObjectImmediate(rule);
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);
        }
    }
}