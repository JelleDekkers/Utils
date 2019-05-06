using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Interface for defining variables needed by <see cref="StateMachineInspector"/>
    /// </summary>
    public interface IInspectable
    {
        /// <summary>
        /// The ScriptableObject that will be inspected
        /// </summary>
        ScriptableObject InspectableObject { get; }
    }
}
