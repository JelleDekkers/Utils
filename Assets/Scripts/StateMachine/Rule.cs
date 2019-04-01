using System;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public abstract class Rule
    {
        /// <summary>
        /// Wether this rule is valid and should transition to the next state
        /// </summary>
        public virtual bool IsValid { get { return false; } }

        /// <summary>
        /// The name that shows in the editor window
        /// </summary>
        public virtual string DisplayName { get { return GetType().Name; } }

        /// <summary>
        /// Called when the state begins
        /// </summary>
        public virtual void OnActivate() { }

        /// <summary>
        /// Called when the state ends
        /// </summary>
        public virtual void OnDeactivate() { }
    }
}