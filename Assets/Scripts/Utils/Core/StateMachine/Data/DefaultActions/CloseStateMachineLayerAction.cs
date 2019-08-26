namespace Utils.Core.Flow.DefaultActions
{
    public class CloseStateMachineLayerAction : StateAction
    {
        private StateMachineLayer layer;

        public void InjectDependencies(StateMachineLayer layer)
        {
            this.layer = layer;
        }

        public override void OnStarted()
        {
            layer.Close();
        }
    }
}