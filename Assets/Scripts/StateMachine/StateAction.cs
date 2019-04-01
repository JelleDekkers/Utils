using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for actions on a <see cref="State"/>, inherits from <see cref=" ScriptableObject"/> to allow for abstract inspector editing
    /// </summary>
    [Serializable]
    public abstract class StateAction : ScriptableObject
    {
        /// <summary>
        /// The name that shows in the editor window
        /// </summary>
        public virtual string DisplayName { get; private set; }

        /// <summary>
        /// Called when the state is starting
        /// </summary>
        public virtual void OnStateStart() { }

        /// <summary>
        /// Called each frame when this state is active
        /// </summary>
        public virtual void OnStateUpdate() { }

        /// <summary>
        /// Called when the state exits
        /// </summary>
        public virtual void OnStateExit() { }

        public virtual void Start()
        {

        }

        public virtual void Stop()
        {

        }
    }
}