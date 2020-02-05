﻿using System;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Abstract class for actions on a <see cref="State"/>, inherits from <see cref=" ScriptableObject"/> to allow for serialization and inspector editing
    /// </summary>
    [Serializable]
    public abstract class StateAction : ScriptableObject
    {
        /// <summary>
        /// Called when the state will be starting, is called before <see cref="OnStopped"/> on the previous state
        /// </summary>
        public virtual void OnStarting() { }

        /// <summary>
        /// Called when the state has started
        /// </summary>
        public virtual void OnStarted() { }

        /// <summary>
        /// Called each frame when this state is active
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called when the state is going to be stopped, is called before <see cref="OnStarted"/> on the new state and before <see cref="OnStopped"/>
        /// </summary>
        public virtual void OnStopping() { }

        /// <summary>
        /// Called when the state has fully stopped
        /// </summary>
        public virtual void OnStopped() { }
    }
}