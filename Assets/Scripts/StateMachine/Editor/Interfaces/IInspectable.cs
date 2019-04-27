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
        /// The type of UI behaviour, should inherit from <see cref="InspectorUIBehaviour"/>
        /// </summary>
        Type InspectorBehaviour { get; }

        /// <summary>
        /// The property name that will be inspected
        /// </summary>
        string PropertyFieldName { get; }

        /// <summary>
        /// The ScriptableObject that will be inspected
        /// </summary>
        ScriptableObject InspectableObject { get; }
    }
}
