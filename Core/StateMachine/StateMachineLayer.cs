using System;
using Utils.Core.Injection;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling <see cref="StateMachineScriptableObjectData"/> logic during runtime such as evaluating <see cref="Rule"/>s and transitioning <see cref=" State"/>s
    /// Layers can be stacked on top of eachother to handle different flows and to prevent merge conflicts with version control
    /// </summary>
    public class StateMachineLayer
    {
        public Action<State, State> StateChangedEvent;

        public IStateMachineData Data { get; private set; }
        public State CurrentState { get; private set; }

        private readonly StateMachine manager;
        private readonly DependencyInjector dependencyInjector;

        public StateMachineLayer(StateMachine manager, IStateMachineData data, DependencyInjector injector = null)
        {
            Data = data;
            this.manager = manager;

            if (data.EntryState != null)
            {
                CurrentState = data.EntryState;
            }

            dependencyInjector = (injector != null) ? injector : new DependencyInjector();
            dependencyInjector.RegisterInstance<StateMachineLayer>(this);
        }

        public void Start(State prevCurrentState = null)
        {
            CurrentState.OnStart();

            foreach (StateAction action in CurrentState.RunTimeActions)
            {
                dependencyInjector.InjectMethod(action);
                action.OnStarting();
            }

            foreach (StateAction action in CurrentState.RunTimeActions)
            {
                action.OnStarted();
            }

            foreach (RuleGroup group in CurrentState.RuleGroups)
            {
                group.OnActivate();

                foreach (Rule rule in group.RuntimeRules)
                {
                    dependencyInjector.InjectMethod(rule);
                    rule.OnActivate();
                }
            }

            StateChangedEvent?.Invoke(prevCurrentState, CurrentState);
        }

        public void Update()
        {
            if (CurrentState == null) { return; }

            for (int i = 0; i < CurrentState.RunTimeActions.Count; i++)
            {
                CurrentState.RunTimeActions[i].Update();
            }

            if (EvaluateStatesForTransition(CurrentState, out State newState, out RuleGroup ruleGroup))
            {
                TransitionToState(CurrentState, newState);

                if (newState == null)
                {
                    StateMachine.PrintDebug(string.Format("{0} Has no destination state!", CurrentState));
                }
            }
        }

        public void AddNewLayer(StateMachineScriptableObjectData data)
        {
            WaitUntillTransitionDone(() => manager.AddNewLayerToStack(data));
        }

        public void Close()
        {
            WaitUntillTransitionDone(manager.PopCurrentLayer);
        }

        private void WaitUntillTransitionDone(Action onDone)
        {
            void OnTransitionDoneEvent(State from, State to)
            {
                StateChangedEvent -= OnTransitionDoneEvent;
                onDone.Invoke();
            };

            StateChangedEvent += OnTransitionDoneEvent;
        }

        private bool EvaluateStatesForTransition(State currentState, out State newState, out RuleGroup validRuleGroup)
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

		public void TransitionToState(State newState)
		{
			TransitionToState(CurrentState, newState);
		}

        private void TransitionToState(State prevState, State newState)
        {
            StateMachine.PrintDebug(string.Format("Transitioning from {0} to {1}", (prevState != null) ? prevState.ToString() : "NONE", (newState != null) ? newState.ToString() : "NONE"));

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
            StateChangedEvent?.Invoke(prevState, newState);
        }

        public void OnClose(State newState = null)
        {
            foreach (StateAction action in CurrentState.RunTimeActions)
            {
                action.OnStopping();
            }

            foreach (StateAction action in CurrentState.RunTimeActions)
            {
                action.OnStopped();
            }

            foreach (RuleGroup group in CurrentState.RuleGroups)
            {
                foreach (Rule rule in group.RuntimeRules)
                {
                    rule.OnDeactivate();
                }

                group.OnDeactivatie();
            }

            CurrentState.OnExit();
            StateChangedEvent?.Invoke(CurrentState, newState);
        }
    }
}
