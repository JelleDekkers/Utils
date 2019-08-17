using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Monobehaviour for using a <see cref="Flow.StateMachineData"/> in the scene
    /// <see cref="StateMachineData"/> will be used as the first <see cref="StateMachineLayer"/> on <see cref="StateMachineLogic"/>
    /// Has DontDestroyOnLoad called on this gameObject
    /// </summary>
    public class StateMachineExecutor : MonoBehaviour
    {
        public StateMachineData StateMachineData => stateMachineData;
        [SerializeField] protected StateMachineData stateMachineData;

        public Statemachine StateMachineLogic { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            StateMachineLogic = new Statemachine(stateMachineData);
        }

        private void Update()
        {
            StateMachineLogic.Update();
        }
    }
}