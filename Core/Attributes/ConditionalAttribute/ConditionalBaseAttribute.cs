using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public abstract class ConditionalBaseAttribute : PropertyAttribute
    {
        //The name of the bool field that will be in control
        public readonly string ConditionalSourceField = "";

        public ConditionalBaseAttribute(string conditionalSourceField)
        {
            ConditionalSourceField = conditionalSourceField;
        }
    }
}