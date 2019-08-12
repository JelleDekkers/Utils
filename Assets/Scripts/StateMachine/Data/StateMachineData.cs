using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// ScriptableObject for StateMachine data, contains <see cref="State"/>s
    /// </summary>
    [CreateAssetMenu(fileName = "New StateMachine Data", menuName = "New StateMachine", order = 1)]
    [Serializable]
    public class StateMachineData : ScriptableObject
    {
        [SerializeField] public State EntryState;
        [SerializeField] public List<State> States = new List<State>();

        public void AddNewState(State state)
        {
            if (States.Count == 0)
            {
                EntryState = state;
            }

            States.Add(state);
        }
    }
}