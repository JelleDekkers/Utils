using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

// TODO: DRY principle for this and scriptableObjectData class

namespace Utils.Core.Flow
{
    /// <summary>
    /// Non ScriptableObject class for StateMachine data, used by <see cref="StateMachineMonoBehaviour"/>
    /// </summary>
    [Serializable]
    public class StateMachineMonoBehaviourData : IStateMachineData
    {
        [SerializeField] private int entryStateID;
        public State EntryState => FindEntryState();

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

        public void AddState(State state)
        {
            if (states.Count == 0)
            {
                SetEntryState(state);
            }

            states.Add(state);
        }

        public void RemoveState(State state)
        {
            throw new NotImplementedException();
        }

        public void SetEntryState(State state)
        {
            entryStateID = state.ID;
        }

        private State FindEntryState()
        {
            foreach(State state in states)
            {
                if (state.ID == entryStateID)
                    return state;
            }

            return null;
        }

        public IStateMachineData Copy()
        {
            return this;
        }

        public State GetStateByID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
