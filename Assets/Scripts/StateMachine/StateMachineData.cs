using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// ScriptableObject for StateMachine data, contains <see cref="State"/>s
    /// </summary>
    [CreateAssetMenu(fileName = "New StateMachine Data", menuName = "New StateMachine", order = 1)]
    [Serializable]
    public class StateMachineData : ScriptableObject
    {
        public State EntryState { get { return entryState; } set { entryState = value; } }
        [SerializeField] private State entryState;

        public List<State> States { get { return states; } set { states = value; } }
        [SerializeField] private List<State> states = new List<State>();

        public void AddNewState(State state)
        {
            if (States.Count == 0)
            {
                entryState = state;
            }

            States.Add(state);
        }
    }
}