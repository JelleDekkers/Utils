namespace Utils.Core.Flow
{
    /// <summary>
    /// Container class for <see cref="StateMachineData"/> and <see cref="currentState"/>. 
    /// Layers can be stacked on top of eachother to handle different flows and to prevent merge conflicts with version control
    /// </summary>
    public class StateMachineLayer
    {
        public StateMachineData Data { get; private set; }

        public State currentState;

        private readonly StateMachineLogic logic;

        public StateMachineLayer(StateMachineData data, StateMachineLogic logic)
        {
            Data = data;
            this.logic = logic;

            currentState = data.EntryState;
        }
    }
}
