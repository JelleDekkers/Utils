using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Monobehaviour for using a <see cref="StateMachine.StateMachineData"/> in the scene. 
    /// Has DontDestroyOnLoad called on this gameObject.
    /// </summary>
    public class StateMachineExecutor : MonoBehaviour
    {
        public StateMachineData StateMachineData => stateMachineData;
        [SerializeField] protected StateMachineData stateMachineData;

        public StateMachineLogic Logic { get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(this);
            Logic = new StateMachineLogic(stateMachineData);
        }

        private void Update()
        {
            Logic.Update();
        }
    }
}