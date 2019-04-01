using System;
using UnityEngine;

/// <summary>
/// Use this property on a <see cref="ScriptableObject"/> type to allow the editors drawing the field to draw an expandable
/// area that allows for changing the values, also works on different inherited types using polymorphism. 
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class InspectableSOAttribute : PropertyAttribute
{
    public InspectableSOAttribute() { }
}