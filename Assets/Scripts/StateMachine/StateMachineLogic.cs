using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Core.Injection;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling <see cref="StateMachineData"/> logic during runtime such as evaluating <see cref="Rule"/>s and transitioning <see cref=" State"/>s
    /// Use scripting define symbol "DEBUG_FLOW" to show debugs
    /// </summary>
    public class StateMachineLogic
    {
        public Action<StateMachineLayer, StateMachineLayer> LayerChangedEvent;
        public Action<State, State> TransitionDoneEvent;

        public StateMachineData CurrentStateMachineData { get { return layerStack.Peek().Data; } }
        public State CurrentState { get { return layerStack.Peek().currentState; } }

        private readonly DependencyInjector dependencyInjector;

        private Stack<StateMachineLayer> layerStack = new Stack<StateMachineLayer>();

        public StateMachineLogic(StateMachineData data)
        {
            layerStack.Push(new StateMachineLayer(data, this));
            dependencyInjector = new DependencyInjector();
            dependencyInjector.RegisterInstance<StateMachineLogic>(this);

            StartStateMachine();
        }

        public void PushStateMachineToStack(StateMachineLayer layer)
        {
            void OnTransitionDoneEvent(State from, State to)
            {
                TransitionDoneEvent -= OnTransitionDoneEvent;

                State prevState = CurrentState;
                StateMachineLayer prevLayer = layerStack.Peek();

                layerStack.Push(layer);
                TransitionToNewLayer(prevState, layer.Data.EntryState);

                LayerChangedEvent?.Invoke(prevLayer, layerStack.Peek());
            };

            TransitionDoneEvent += OnTransitionDoneEvent;
        }

        public void PopCurrentStateMachineFromStack()
        {
            void OnTransitionDoneEvent(State from, State to)
            {
                TransitionDoneEvent -= OnTransitionDoneEvent;
                StateMachineLayer prevData = layerStack.Pop();
                TransitionFromPoppedLayer(prevData.currentState, CurrentState);

                LayerChangedEvent?.Invoke(prevData, layerStack.Peek());
            };

            TransitionDoneEvent += OnTransitionDoneEvent;
        }

        private void StartStateMachine()
        {
            TransitionToState(null, CurrentStateMachineData.EntryState);
        }

        public void Update()
        {
            if(CurrentState == null) { return; }

            if (EvaluateStatesForTransition(CurrentState, out State newState, out RuleGroup ruleGroup))
            {
                TransitionToState(CurrentState, newState);

                if(newState == null)
                {
                    DebugLog(string.Format("{0} Has no destination state!", CurrentState));
                }
            }

            for (int i = 0; i < CurrentState.RunTimeActions.Count; i++)
            {
                CurrentState.RunTimeActions[i].Update();
            }
        }

        private void TransitionToNewLayer(State prevState, State newState)
        {
            DebugLog("starting state on new layer " + newState);

            newState.OnStart();

            foreach (StateAction action in newState.RunTimeActions)
            {
                dependencyInjector.InjectMethod(action);
                action.OnStarting();
            }
           
            foreach (StateAction action in newState.RunTimeActions)
            {
                action.OnStarted();
            }

            foreach (RuleGroup group in newState.RuleGroups)
            {
                group.OnActivate();

                foreach (Rule rule in group.RuntimeRules)
                {
                    dependencyInjector.InjectMethod(rule);
                    rule.OnActivate();
                }
            }

            layerStack.Peek().currentState = newState;
            TransitionDoneEvent?.Invoke(prevState, newState);
        }

        private void TransitionFromPoppedLayer(State prevState, State newState)
        {
            DebugLog(string.Format("LAYER POPPED, Transitioning from {0} to {1}", (prevState != null) ? prevState.ToString() : "NONE", (newState != null) ? newState.ToString() : "NONE"));

            foreach (StateAction action in prevState.RunTimeActions)
            {
                action.OnStopping();
            }
        
            foreach (StateAction action in prevState.RunTimeActions)
            {
                action.OnStopped();
            }

            foreach (RuleGroup group in prevState.RuleGroups)
            {
                foreach (Rule rule in group.RuntimeRules)
                {
                    rule.OnDeactivate();
                }

                group.OnDeactivatie();
            }

            prevState.OnExit();

            layerStack.Peek().currentState = newState;
            TransitionDoneEvent?.Invoke(prevState, newState);
        }


        private void TransitionToState(State prevState, State newState)
        {
            DebugLog(string.Format("Transitioning from {0} to {1}", (prevState != null) ? prevState.ToString() : "NONE", (newState != null) ? newState.ToString() : "NONE"));

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

            layerStack.Peek().currentState = newState;
            TransitionDoneEvent?.Invoke(prevState, newState);
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

        /// <summary>
        /// Prints Debug.Log if DEBUG_FLOW is found in the projects Scripting Define Symbols
        /// </summary>
        /// <param name="log"></param>
        [System.Diagnostics.Conditional("DEBUG_FLOW")]
        public static void DebugLog(string log)
        {
            Debug.Log(log);
        }
    }
}