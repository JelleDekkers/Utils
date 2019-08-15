namespace Utils.Core.Flow.DefaultActions
{
    public class CloseStateMachineAction : StateAction
    {
        private StateMachineLogic logic;

        public void InjectDependencies(StateMachineLogic logic)
        {
            this.logic = logic;
        }

        public override void OnStarted()
        {
            logic.PopCurrentStateMachineFromStack();
        }
    }
}