using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Attribute for using a custom class on <see cref="StateMachineInspector"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomInspectorUIAttribute : Attribute
    {
        public Type InspectorTargetType { get; private set; }

        public CustomInspectorUIAttribute(Type inspectorTargetType = null)
        {
            InspectorTargetType = inspectorTargetType;
        }
    }
}
