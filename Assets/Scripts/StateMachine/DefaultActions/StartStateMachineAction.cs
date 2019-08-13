using UnityEngine;

namespace Utils.Core.Flow.DefaultActions
{
    public class StartStateMachineAction : StateAction
    {
        [SerializeField] private StateMachineData stateMachine = null;

        public override void OnStarted()
        {

        }
    }
}
