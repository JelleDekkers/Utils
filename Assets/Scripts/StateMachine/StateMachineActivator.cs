using UnityEngine;

namespace StateMachine
{
    public class StateMachineActivator : MonoBehaviour
    {
        public StateMachine StateMachine { get { return stateMachine; } }
        [SerializeField] protected StateMachine stateMachine;

        private void Start()
        {
            DontDestroyOnLoad(this);
            StateMachine.Run(this);
        }
    }
}