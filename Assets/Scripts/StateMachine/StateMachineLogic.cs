using UnityEngine;
using Utils.Core.Injection;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling <see cref="StateMachineData"/> logic during runtime such as evaluating <see cref="Rule"/>s and transitioning <see cref=" State"/>s
    /// </summary>
    public class StateMachineLogic
    {
        public StateMachineData Data { get; private set; }
        public State CurrentState { get; private set; }

        private readonly DependencyInjector dependencyInjector;

        public StateMachineLogic(StateMachineData data)
        {
            Data = data;
            dependencyInjector = new DependencyInjector();

            StartStateMachine();
        }

        private void StartStateMachine()
        {
            CurrentState = Data.EntryState;
            TransitionToState(null, CurrentState);
        }

        public void Update()
        {
            if(CurrentState == null) { return; }

            if (StateCanTransitionToNextState(CurrentState, out State newState, out RuleGroup ruleGroup))
            {
                TransitionToState(CurrentState, newState);

                if(newState == null)
                { 
                    Debug.LogWarningFormat("{0} Has no destination state!", CurrentState);
                }
            }

            for (int i = 0; i < CurrentState.RunTimeActions.Count; i++)
            {
                CurrentState.RunTimeActions[i].Update();
            }
        }

        private void TransitionToState(State prevState, State newState)
        {
            Debug.LogFormat("Transitioning from {0} to {1}", (prevState != null) ? prevState.ToString() : "NONE", (newState != null) ? newState.ToString() : "NONE");

            if (newState != null)
            {
                newState.OnStart();

                // newState OnStarting
                foreach (StateAction action in newState.RunTimeActions)
                {
                    dependencyInjector.InjectMethod(action);
                    action.OnStarting();
                }
            }

            if (prevState != null)
            {
                // prevState OnStopping
                foreach (StateAction action in prevState.RunTimeActions)
                {
                    action.OnStopping();
                }
            }

            if (newState != null)
            {
                // newState OnStarted
                foreach (StateAction action in newState.RunTimeActions)
                {
                    action.OnStarted();
                }

                // newState Rules OnActivate 
                foreach (RuleGroup group in newState.RuleGroups)
                {
                    group.OnActivate();

                    foreach (Rule rule in group.RuntimeRules)
                    {
                        dependencyInjector.InjectMethod(rule);
                        rule.OnActivate();
                    }
                }
            }

            if (prevState != null)
            {
                // prevState OnStopped
                foreach (StateAction action in prevState.RunTimeActions)
                {
                    action.OnStopped();
                }
         
                // prevState Rules OnDeactivate
                foreach (RuleGroup group in prevState.RuleGroups)
                {
                    foreach (Rule rule in group.RuntimeRules)
                    {
                        rule.OnDeactivate();
                    }

                    group.OnDeactivatie();
                }

                prevState.OnExit();
            }

            CurrentState = newState;
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