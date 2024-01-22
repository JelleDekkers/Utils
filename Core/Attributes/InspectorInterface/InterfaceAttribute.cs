using UnityEngine;
using System;

namespace Utils.Core.Attributes
{
    /// <summary>
    /// When this attribute is attached to a MonoBehaviour field within a
    /// Unity Object, this allows an interface to be specified in to to
    /// entire only a specific type of MonoBehaviour can be attached.
    /// </summary>
    public class InterfaceAttribute : PropertyAttribute
    {
        public Type[] Types = null;
        public string TypeFromFieldName;

        /// <summary>
        /// Creates a new Interface attribute.
        /// </summary>
        /// <param name="type">The type of interface which is allowed.</param>
        public InterfaceAttribute(Type type, params Type[] types)
        {
            Types = new Type[types.Length + 1];
            Types[0] = type;
            for (int i = 0; i < types.Length; i++)
            {
                Types[i + 1] = types[i];
            }
        }

        public InterfaceAttribute(string typeFromFieldName)
        {
            this.TypeFromFieldName = typeFromFieldName;
        }
    }
}
