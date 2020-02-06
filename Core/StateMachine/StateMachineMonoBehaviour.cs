using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// State Machine for local scene references. Does not work with assets and will therefore always have only one instance available <see cref="StateMachineLayer"/>
    /// </summary>
    public class StateMachineMonoBehaviour : MonoBehaviour
    {
        public StateMachine StateMachine { get; private set; }

        [HideInInspector] public StateMachineMonoBehaviourData Data;

        private void Awake()
        {
            if (Data != null)
            {
                StateMachine = new StateMachine(Data);
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
