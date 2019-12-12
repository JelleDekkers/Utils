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
		public readonly Type constType;

		public ConstantTypeReferenceAttribute(Type type, Type constType)
		{
			this.constType = constType;
		}
	}
}
