using UnityEngine;

namespace NodeEditor
{
    public class StateManager : MonoBehaviour
    {
        public float health;
        public State currentState;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public new Transform transform;

        private void Start()
        {
            transform = gameObject.transform; 
        }

        private void Update()
        {
            if(currentState != null)
            {
                currentState.Tick(this);
            }
        }
    }
}