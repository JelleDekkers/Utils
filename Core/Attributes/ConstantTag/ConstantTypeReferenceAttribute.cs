using System;
using UnityEngine;

namespace Utils.Core
{
	/// <summary>
	/// Attribute for showing constant fields of a certain type in the editor
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ConstantTypeReferenceAttribute : PropertyAttribute
	{
        public readonly Type type;

        /// <summary>
        /// Attribute for showing constant fields of a certain type in the editor
        /// </summary>
        /// <param name="type">The object on which the constants are specified</param>
		public ConstantTypeReferenceAttribute(Type type)
		{
            this.type = type;
		}
	}
}
