using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for handling <see cref="StateMachineData"/> logic during runtime
    /// </summary>
    public class StateMachineLogic
    {
        public StateMachineData Data => data;
        [SerializeField] private StateMachineData data;

        public State CurrentState { get; private set; }

        public StateMachineLogic(StateMachineData data)
        {
            this.data = data;
            Initialize();
        }

        private void Initialize()
        {
            CurrentState = data.EntryState;
            OnStateStart(CurrentState);
        }

        public void Update()
        {
            if(CurrentState == null) { return; }

            if (StateCanTransitionToNextState(CurrentState, out State newState, out RuleGroup ruleGroup))
            {
                Debug.LogFormat("Changing from {0} to {1}", CurrentState, (newState != null) ? newState.ToString() : "NONE");

                OnStateStop(CurrentState);
                CurrentState = newState;

                if (newState != null)
                {
                    OnStateStart(CurrentState);
                }
                else
                {
                    Debug.LogWarningFormat("{0} Has no destination state!", CurrentState);
                }
            }
        }

        private void OnStateStart(State state)
        {
            foreach (StateAction action in state.Actions)
            {
                action.Start();
            }

            foreach(RuleGroup group in state.RuleGroups)
            {
                foreach(Rule rule in group.Rules)
                {
                    rule.OnActivate();
                }
            }
        }

        private void OnStateStop(State state)
        {
            foreach (StateAction action in state.Actions)
            {
                action.Stop();
            }

            foreach (RuleGroup group in state.RuleGroups)
            {
                foreach (Rule rule in group.Rules)
                {
                    rule.OnDeactivate();
                }
            }
        }

        private bool StateCanTransitionToNextState(State currentState, out State newState, out RuleGroup validRuleGroup)
        {
            foreach (RuleGroup ruleGroup in currentState.RuleGroups)
            {
                if (ruleGroup.AllRulesAreValid())
                {
                    newState = ruleGroup.Destination;
                    validRuleGroup = ruleGroup;
                    return true;
                }
            }

            newState = null;
            validRuleGroup = null;
            return false;
        }
    }
}