using UnityEngine;

namespace Utils.Core.Flow.DefaultActions
{
    public class StartStateMachineAction : StateAction
    {
        [SerializeField] private StateMachineData stateMachine = null;

        private StateMachineLogic logic;

        public void InjectDependencies(StateMachineLogic logic)
        {
            this.logic = logic; 
        }

        public override void OnStarted()
        {
            logic.PushStateMachineToStack(new StateMachineLayer(stateMachine, logic));
        }
    }
}
