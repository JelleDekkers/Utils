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
        public State EntryState { get { return entryState; } }
        [SerializeField] private State entryState;

        public List<State> States => states;
        [SerializeField] private List<State> states = new List<State>();

        public void AddNewState(State state)
        {
            if (States.Count == 0)
            {
                SetEntryState(state);
            }

            States.Add(state);
        }

        public void RemoveState(State state)
        {
            States.Remove(state);
        }

        public void Clear()
        {
            States.Clear();
        }

        public void SetEntryState(State state)
        {
            entryState = state; 
        }
    }
}