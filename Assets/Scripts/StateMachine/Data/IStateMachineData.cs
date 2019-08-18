using System.Collections.Generic;

namespace Utils.Core.Flow
{
    public interface IStateMachineData
    {
        State EntryState { get; set; }
        List<State> States { get; set; }
        string Name { get; }

        void AddNewState(State state);
    }
}
