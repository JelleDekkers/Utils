using System.Collections.Generic;

namespace Utils.Core.Flow
{
    public interface IStateMachineData
    {
        State EntryState { get; }
        List<State> States { get; }
        string Name { get; }

        /// <summary>
        /// The object/asset Unity uses for serialization 
        /// </summary>
        UnityEngine.Object SerializedObject { get; }

        void AddState(State state);
        void RemoveState(State state);
        State GetStateByID(int id);
        IStateMachineData Copy();
    }
}
