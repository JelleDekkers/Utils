using System;

namespace Utils.Core.Flow.Inspector
{
    /// <summary>
    /// Attribute for using a custom class on <see cref="StateMachineInspector"/>
    /// <see cref="InspectorTargetType"/> is the inspected Type for which the custom inspector is needed
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
