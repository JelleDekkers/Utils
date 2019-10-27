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

        public StateMachine StateMachine { get; protected set; }

        private void Awake()
        {
			DontDestroyOnLoad(this);
		}

		private void Start()
		{
			Initialize();
			StateMachine?.Start();
		}

		protected virtual void Initialize()
		{
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