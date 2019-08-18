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
            if (data is StateMachineScriptableObjectData)
            {
                ClearStateMachine(data);
            }
            else if (data is StateMachineMonoBehaviourData)
            {
                ClearStateMachine(data as StateMachineMonoBehaviourData);
            }

            Debug.LogWarning("Type not found");
        }

        public static void ClearStateMachine(this StateMachineScriptableObjectData data)
        {
            Undo.RecordObject(data, "Clear Machine");

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
            EditorUtility.SetDirty(data);
        }

        public static void ClearStateMachine(this StateMachineMonoBehaviourData data)
        {
            //Undo.RecordObject(data, "Clear Machine");

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
            //EditorUtility.SetDirty(data);
        }

        public static State CreateNewState(this IStateMachineData data, Vector2 position)
        {
            if(data is StateMachineScriptableObjectData)
            {
                return CreateNewState(data as StateMachineScriptableObjectData, position);
            }
            else if(data is StateMachineMonoBehaviourData)
            {
                return CreateNewState(data as StateMachineMonoBehaviourData, position);
            }

            Debug.LogWarning("Type not found");
            return null;
        }

        public static State CreateNewState(this StateMachineScriptableObjectData data, Vector2 position)
        {
            Undo.RecordObject(data, "Add State");

            string assetFilePath = AssetDatabase.GetAssetPath(data);
            State state = StateMachineEditorUtilityHelper.CreateObjectInstance<State>(assetFilePath);
            state.Position = position;
            data.AddNewState(state);

            StateAddedEvent?.Invoke(data, state);
            EditorUtility.SetDirty(data);

            return state;
        }

        public static State CreateNewState(this StateMachineMonoBehaviourData data, Vector2 position)
        {
            //Undo.RecordObject(stateMachine, "Add State");

            State state = ScriptableObject.CreateInstance<State>();
            state.Position = position;
            data.AddNewState(state);

            StateAddedEvent?.Invoke(data, state);
            //EditorUtility.SetDirty(stateMachine);

            return state;
        }

        public static void RemoveState(this IStateMachineData stateMachine, State state)
        {
            StateRemovedEvent?.Invoke(stateMachine, state);

            //Undo.RecordObject(stateMachine, "Remove State");

            bool isEntryState = stateMachine.EntryState == state;

            stateMachine.States.Remove(state);
            Object.DestroyImmediate(state, true);

            if (isEntryState && stateMachine.States.Count > 0)
            {
                SetEntryState(stateMachine, stateMachine.States[0]);
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

            //EditorUtility.SetDirty(stateMachine);
        }

        public static void SetEntryState(this IStateMachineData stateMachine, State state)
        {
            //Undo.RecordObject(stateMachine, "Set Entry State");
            Undo.RecordObject(state, "Set Entry State");
            stateMachine.EntryState = state;
            EditorUtility.SetDirty(state);
            //EditorUtility.SetDirty(stateMachine);
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

        public static void AddStateAction(this State state, Type type, bool createAsset)
        {
            Undo.RecordObject(state, "Add Action");

            StateAction stateAction = null;
            if (createAsset)
            {
                string assetFilePath = AssetDatabase.GetAssetPath(state);
                stateAction = StateMachineEditorUtilityHelper.CreateObjectInstance(type, assetFilePath) as StateAction;
            }
            else
            {
                stateAction = ScriptableObject.CreateInstance(type) as StateAction;
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

        public static void Clear(this RuleGroup ruleGroup)
        {
            // TODO: needs reference to the statemachine?
            //Undo.RecordObject(ruleGroup, "Clear RuleGroup");

            foreach(Rule rule in ruleGroup.TemplateRules)
            {
                Object.DestroyImmediate(rule, true);
            }
            ruleGroup.TemplateRules.Clear();

            //EditorUtility.SetDirty(ruleGroup);
        }

        public static void Reset(this Rule rule, RuleGroup ruleGroup)
        {
            // TODO: needs reference to the statemachine?
            //Undo.RecordObject(ruleGroup, "Reset Action");

            string assetFilePath = AssetDatabase.GetAssetPath(rule);
            Rule newRule = StateMachineEditorUtilityHelper.CreateObjectInstance(rule.GetType(), assetFilePath) as Rule;
            int index = ruleGroup.TemplateRules.IndexOf(rule);
            Object.DestroyImmediate(rule, true);
            ruleGroup.TemplateRules[index] = newRule;

            ObjectResetEvent?.Invoke(rule);
            //EditorUtility.SetDirty(ruleGroup);
        }

        public static void AddNewRule(this RuleGroup group, IStateMachineData data, Type ruleType)
        {
            if (data is StateMachineScriptableObjectData)
            {
                AddNewRule(group, data as StateMachineScriptableObjectData, ruleType);
            }
            else if (data is StateMachineMonoBehaviourData)
            {
                AddNewRule(group, data as StateMachineMonoBehaviourData, ruleType);
            }

            Debug.LogWarning("Type not found");
        }

        public static void AddNewRule(this RuleGroup group, StateMachineMonoBehaviourData stateMachine, Type ruleType)
        {
            Rule rule = ScriptableObject.CreateInstance(ruleType) as Rule;
            group.TemplateRules.Add(rule);
        }

        public static void AddNewRule(this RuleGroup group, StateMachineScriptableObjectData stateMachine, Type ruleType)
        {
            // TODO: needs reference to the statemachine?
            //Undo.RecordObject(group, "Add Rule");
            string assetFilePath = AssetDatabase.GetAssetPath(stateMachine);
            Rule rule = StateMachineEditorUtilityHelper.CreateObjectInstance(ruleType, assetFilePath) as Rule;
            group.TemplateRules.Add(rule);

            //EditorUtility.SetDirty(group);
        }

        public static void RemoveRule(this RuleGroup group, Rule rule)
        {
            // TODO: needs reference to the statemachine?
            //Undo.RecordObject(group, "Remove rule");
            group.TemplateRules.Remove(rule);
            Object.DestroyImmediate(rule, true);
            //EditorUtility.SetDirty(group);
        }
    }
}