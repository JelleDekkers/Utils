using UnityEngine;

namespace Utils.Core.Flow.DefaultActions
{
    public class StartStateMachineLayerAction : StateAction
    {
        [SerializeField] private StateMachineScriptableObjectData stateMachine = null;

        private StateMachineLayer layer;

        public void InjectDependencies(StateMachineLayer layer)
        {
            this.layer = layer; 
        }

        public override void OnStarted()
        {
            layer.AddNewLayer(stateMachine);
        }
    }
}
