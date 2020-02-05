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
        [SerializeField] private int entryStateID;
        public State EntryState => FindEntryState();

        [SerializeField] private List<State> states = new List<State>();
        public List<State> States
        {
            get { return states; }
            set { states = value; }
        }

        public string Name => GetType().Name;
        public UnityEngine.Object SerializedObject => this;

        private Dictionary<int, State> statesTable;

        private void Initialize()
        {
            foreach (State state in states)
            {
                state.Initialize();
            }

            UpdateStatesTable();
        }

        public void AddState(State state)
        {
            if (states.Count == 0)
            {
                SetEntryState(state);
            }

            states.Add(state);
            statesTable.Add(state.ID, state);
            UpdateStatesTable();
        }

        public void SetEntryState(State state)
        {
            entryStateID = state.ID;
        }

        // TODO: use either this one or at statemachineEditorUtility
        public void RemoveState(State state)
        {
            states.Remove(state);

            if (statesTable.ContainsKey(state.ID))
            {
                statesTable.Add(state.ID, state);
                UpdateStatesTable();
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
            if(statesTable == null)
            {
                UpdateStatesTable();
            }

            return statesTable[id];
        }

        private void UpdateStatesTable()
        {
            statesTable = new Dictionary<int, State>();
            foreach (State state in states)
            {
                statesTable.Add(state.ID, state);
            }
        }

        public IStateMachineData Copy()
        {
            StateMachineScriptableObjectData clone = Instantiate(this);
            clone.Initialize();
            return clone;
        }
    }
}
