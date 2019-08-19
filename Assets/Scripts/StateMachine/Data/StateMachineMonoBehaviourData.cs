using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Non ScriptableObject class for StateMachine data, used by <see cref="StateMachineMonoBehaviour"/>
    /// </summary>
    [Serializable]
    public class StateMachineMonoBehaviourData : IStateMachineData
    { 
        [SerializeField] private State entryState;
        public State EntryState
        {
            get { return entryState; }
            set { entryState = value; }
        }

        [SerializeField] private List<State> states = new List<State>();
        public List<State> States
        {
            get { return states; }
            set { states = value; }
        }

        public string Name => GetType().Name;

        [SerializeField] private Object obj; 
        public Object SerializedObject => obj;

        public StateMachineMonoBehaviourData(StateMachineMonoBehaviour gameObject)
        {
            obj = gameObject;
        }

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
