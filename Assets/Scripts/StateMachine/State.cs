using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachineData"/>
    /// </summary>
    [Serializable]
    public class State : ScriptableObject
    {
        public List<StateAction> Actions => actions;
        [SerializeField] private List<StateAction> actions = new List<StateAction>();

        public List<RuleGroup> RuleGroups => ruleGroups;
        [SerializeField] private List<RuleGroup> ruleGroups = new List<RuleGroup>();

        // Editor stuff:
        public string Title = "New State";
        public Rect Rect;

        public void AddAction(StateAction action)
        {
            actions.Add(action);
        }

        public void RemoveAction(StateAction action)
        {
            actions.Remove(action);
        }

        public void AddRuleGroup(RuleGroup ruleGroup)
        {
            ruleGroups.Add(ruleGroup);
        }

        public void RemoveRuleGroup(RuleGroup ruleGroup)
        {
            ruleGroups.Remove(ruleGroup);
        }

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }
    }
}