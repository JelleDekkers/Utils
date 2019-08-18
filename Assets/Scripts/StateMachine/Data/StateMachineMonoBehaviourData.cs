using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Non ScriptableObject class for StateMachine data, used by <see cref="StateMachineMonoBehaviour"/>
    /// </summary>
    [Serializable]
    public class StateMachineMonoBehaviourData : IStateMachineData
    { 
        [SerializeField] public State entryState;
        public State EntryState
        {
            get { return entryState; }
            set { entryState = value; }
        }

        [SerializeField] public List<State> states = new List<State>();
        public List<State> States
        {
            get { return states; }
            set { states = value; }
        }

        public string Name => GetType().Name;

        public void AddNewState(State state)
        {
            if (states.Count == 0)
            {
                entryState = state;
            }

            states.Add(state);
        }
    }
}
