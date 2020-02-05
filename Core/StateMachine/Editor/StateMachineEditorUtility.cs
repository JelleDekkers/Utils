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
        public static Action<ScriptableObject> ObjectResetEvent;
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
            //if (data.SerializedObject is ScriptableObject)
            //{
            //    string assetFilePath = AssetDatabase.GetAssetPath(data.SerializedObject);
            //    state = StateMachineEditorUtilityHelper.CreateObjectInstance<State>(assetFilePath);
            //}
            //else
            //{
            //    state = ScriptableObject.CreateInstance<State>();
            //}

            state.Position = position;
            data.AddState(state);

            StateAddedEvent?.Invoke(data, state);
            EditorUtility.SetDirty(data.SerializedObject);

            return state;
        }

        public static void RemoveState(this IStateMachineData data, State state, bool callEvent = true)
        {
            if (callEvent)
            {
                StateRemovedEvent?.Invoke(data, state);
            }

            Undo.RecordObject(data.SerializedObject, "Remove State");

            bool isEntryState = data.EntryState == state;

            data.States.Remove(state);
            //Undo.DestroyObjectImmediate(state);

            if (isEntryState && data.States.Count > 0)
            {
                data.SetEntryState(data.States[0]);
            }

            foreach(StateAction action in state.Actions)
            {
                Undo.DestroyObjectImmediate(action);
            }

            foreach (RuleGroup ruleGroup in state.RuleGroups)
            {
                foreach(Rule rule in ruleGroup.Rules)
                {
                    Undo.DestroyObjectImmediate(rule);
                }
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void SetEntryState(this IStateMachineData data, State state)
        {
            Undo.RecordObject(data.SerializedObject, "Set Entry State");
            data.SetEntryState(state);
            EditorUtility.SetDirty(data.SerializedObject);
        }

        public static void ClearActions(this State state)
        {
            //Undo.RegisterCompleteObjectUndo(state, "Clear State Actions");

            for (int i = state.Actions.Count - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(state.Actions[i]);
            }

            state.Actions.Clear();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            //EditorUtility.SetDirty(state);
        }

        public static void ClearRules(this State state)
        {
            //Undo.RegisterCompleteObjectUndo(state, "Clear State Rules");

            for (int i = 0; i < state.RuleGroups.Count; i++)
            {
                foreach (Rule rule in state.RuleGroups[i].Rules)
                {
                    Undo.DestroyObjectImmediate(rule);
                }
            }

            state.RuleGroups.Clear();

            //EditorUtility.SetDirty(state);
        }

        public static void AddStateAction(this State state, Type actionType, IStateMachineData data)
        {
            //Undo.RecordObject(state, "Add Action");

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

            state.Actions.Add(stateAction);

            //EditorUtility.SetDirty(state);
        }

        public static void RemoveStateAction(this State state, StateAction stateAction)
        {
            //Undo.RecordObject(state, "Remove Action");

            state.Actions.Remove(stateAction);
            Undo.DestroyObjectImmediate(stateAction);
            //EditorUtility.SetDirty(state);
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

        public static RuleGroup AddNewRuleGroup(this State state)
        {
            //Undo.RecordObject(state, "Add RuleGroup");

            RuleGroup group = new RuleGroup();
            state.RuleGroups.Add(group);

            RuleGroupAddedEvent?.Invoke(state, group);
            //EditorUtility.SetDirty(state);

            return group;
        }

        public static void RemoveRuleGroup(this State state, RuleGroup group)
        {
            //Undo.RecordObject(state, "Remove RuleGroup");

            foreach(Rule rule in group.Rules)
            {
                Object.DestroyImmediate(rule, true);
            }

            state.RuleGroups.Remove(group);
            RuleGroupRemovedEvent?.Invoke(state, group);

            //EditorUtility.SetDirty(state);
        }

        public static void Clear(this RuleGroup ruleGroup, State state)
        {
            //Undo.RegisterCompleteObjectUndo(state, "Clear RuleGroup");

            for (int i = ruleGroup.Rules.Count - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(ruleGroup.Rules[i]);
            }

            ruleGroup.Rules.Clear();

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            //EditorUtility.SetDirty(state);
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

        public static void AddNewRule(this RuleGroup group, Type ruleType, IStateMachineData data, State state)
        {
            //Undo.RecordObject(state, "Add Rule");

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
            group.Rules.Add(rule);

            //EditorUtility.SetDirty(state);
        }

        public static void RemoveRule(this RuleGroup group, Rule rule, State state)
        {
            //Undo.RecordObject(state, "Remove rule");
            group.Rules.Remove(rule);
            Undo.DestroyObjectImmediate(rule);
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            //EditorUtility.SetDirty(state);
        }
    }
}