using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Interface for defining variables needed by <see cref="StateMachineInspector"/>
    /// </summary>
    public interface IInspectable
    {
        /// <summary>
        /// The ScriptableObject that will be inspected
        /// </summary>
        ScriptableObject Target { get; }
        string PropertyName { get; }
    }
}
