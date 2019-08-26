using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// ScriptableObject for StateMachine data, used by <see cref="StateMachineExecutor"/>
    /// </summary>
    [CreateAssetMenu(fileName = "New StateMachine Data", menuName = "New StateMachine", order = 1)]
    [Serializable]
    public class StateMachineScriptableObjectData : ScriptableObject, IStateMachineData
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

        public string Name => name;
        public UnityEngine.Object SerializedObject => this;

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