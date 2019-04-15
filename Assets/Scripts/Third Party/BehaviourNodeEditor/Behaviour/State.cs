using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    [CreateAssetMenu]
    public class State : ScriptableObject
    {
        public StateActions[] onState;
        public StateActions[] onEnter;
        public StateActions[] onExit;
        public List<Transition> transitions = new List<Transition>();

        public void OnEnter(StateManager stateManager)
        {
            ExecuteActions(stateManager, onEnter);
        }

        public void Tick(StateManager stateManager)
        {
            ExecuteActions(stateManager, onState);
            CheckTransitions(stateManager);
        }

        public void OnExit(StateManager stateManager)
        {
            ExecuteActions(stateManager, onExit);
        }

        public void CheckTransitions(StateManager stateManager)
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].disable) continue;

                if (transitions[i].condition.CheckCondition(stateManager))
                {
                    if (transitions[i].targetState != null)
                    {
                        stateManager.currentState = transitions[i].targetState;
                        OnExit(stateManager);
                        stateManager.currentState.OnEnter(stateManager);
                    }

                    return;
                }
            }
        }

        public void ExecuteActions(StateManager stateManager, StateActions[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if(onState[i] != null)
                {
                    onState[i].Execute(stateManager);
                }
            }
        }

        public Transition AddTransition()
        {
            Transition transition = new Transition();
            transitions.Add(transition);

            return transition;
        }
    }
}