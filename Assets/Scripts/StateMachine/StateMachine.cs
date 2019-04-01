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

        public List<State> States /*{ get; private set; } */= new List<State>();

        public void Run(StateMachineActivator activator)
        {
            Debug.Log("running");
            Debug.Log(startState);
            Debug.Log(States.Count);

            foreach (State s in States)
                Debug.Log(s.Title);

            startState.Run();
        }

        public State CreateNewState()
        {
            State state = new State();// CreateInstance<State>();

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
            //DestroyImmediate(state);
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