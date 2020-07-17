using UnityEngine;

namespace Utils.Core.Flow.DefaultActions
{
    public class StartStateMachineLayerAction : StateAction
    {
        [SerializeField] private StateMachineScriptableObjectData stateMachine = null;

        [Tooltip("Should this layer have it's own local EventDispatcher or reuse the current one")]
        [SerializeField] private bool createLocalEventDispatcher = true;

        private StateMachineLayer layer;

        public void InjectDependencies(StateMachineLayer layer)
        {
            this.layer = layer; 
        }

        public override void OnStarted()
        {
            layer.CreateNewLayer(stateMachine, createLocalEventDispatcher);
        }
    }
}
