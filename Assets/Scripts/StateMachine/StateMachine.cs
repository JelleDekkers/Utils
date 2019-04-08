using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// ScriptableObject StateMachine, contains <see cref="State"/>s
    /// </summary>
    [CreateAssetMenu(fileName = "New StateMachine", menuName = "New StateMachine", order = 1)]
    [Serializable]
    public class StateMachine : ScriptableObject
    {
        public State StartState { get { return startState; } }
        [SerializeField] private State startState;

        public List<State> States => states;
        [SerializeField] private List<State> states = new List<State>();

        public void Run(StateMachineActivator activator)
        {
            startState.Run();
        }

        public State CreateNewState()
        {
            State state = new State();

            if (States.Count == 0)
            {
                SetStartingState(state);
            }

            States.Add(state);

            return state;
        }

        public void RemoveState(State state)
        {
            States.Remove(state);
        }

        public void Clear()
        {
            States.Clear();
        }

        public void SetStartingState(State state)
        {
            startState = state;
        }
    }
}