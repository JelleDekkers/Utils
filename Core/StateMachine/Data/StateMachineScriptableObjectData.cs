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
        public State EntryState => FindEntryState();
        [SerializeField] private int entryStateID;

        public List<State> States => states;
        [SerializeField] private List<State> states = new List<State>();

        public string Name => GetType().Name;
        public UnityEngine.Object SerializedObject => this;

        private Dictionary<int, State> statesLookupTable;

        public void AddState(State state)
        {
            if (states.Count == 0)
            {
                SetEntryState(state);
            }

            if(statesLookupTable == null)
            {
                RefreshStatesTable();
            }

            states.Add(state);
            statesLookupTable.Add(state.ID, state);
        }

        public void SetEntryState(State state)
        {
            entryStateID = state.ID;
        }

        public void RemoveState(State state)
        {
            if (statesLookupTable == null)
            {
                RefreshStatesTable();
            }

            if (statesLookupTable.ContainsKey(state.ID))
            {
                statesLookupTable.Remove(state.ID);
            }

            bool wasEntryState = state.ID == entryStateID;
            states.Remove(state);
            if (wasEntryState)
            {
                if (States.Count > 0)
                {
                    SetEntryState(States[0]);
                }
                else
                {
                    entryStateID = -1;
                }
            }
        }

        private State FindEntryState()
        {
            foreach (State state in states)
            {
                if (state.ID == entryStateID)
                    return state;
            }

            return null;
        }

        public State GetStateByID(int id)
        {
            if(statesLookupTable == null)
            {
                RefreshStatesTable();
            }

            return statesLookupTable[id];
        }

        private void RefreshStatesTable()
        {
            statesLookupTable = new Dictionary<int, State>();
            foreach (State state in states)
            {
                statesLookupTable.Add(state.ID, state);
            }
        }

        public IStateMachineData Copy()
        {
            StateMachineScriptableObjectData clone = Instantiate(this);
            return clone;
        }
    }
}
