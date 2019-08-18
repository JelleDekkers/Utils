using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// State Machine for local scene references. Does not work on assets and will therefore always be 1 <see cref="StateMachineLayer"/>
    /// </summary>
    public class StateMachineMonoBehaviour : MonoBehaviour
    {
        public Statemachine StateMachine { get; private set; }

        [HideInInspector] public StateMachineMonoBehaviourData Data;

        private void Awake()
        {
            StateMachine = new Statemachine(Data);
        }

        private void Update()
        {
            StateMachine.Update();
        }
    }
}
