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

        private StateMachineLogic logic;

        private void Start()
        {
            DontDestroyOnLoad(this);
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            logic = new StateMachineLogic(stateMachineData);
        }

        private void Update()
        {
            logic.Update();
        }
    }
}