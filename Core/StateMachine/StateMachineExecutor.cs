using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Monobehaviour for using a <see cref="Flow.StateMachineScriptableObjectData"/> in the scene
    /// <see cref="StateMachineData"/> will be used as the first <see cref="StateMachineLayer"/> on <see cref="StateMachine"/>
    /// Has DontDestroyOnLoad called on this gameObject
    /// </summary>
    public class StateMachineExecutor : MonoBehaviour
    {
        public StateMachineScriptableObjectData StateMachineData => stateMachineData;
        [SerializeField] protected StateMachineScriptableObjectData stateMachineData;

        public StateMachine StateMachine { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (stateMachineData != null)
            {
                StateMachine = new StateMachine(stateMachineData);
            }
            else
            {
                enabled = false;
            }
        }

        private void Update()
        {
            StateMachine.Update();
        }
    }
}