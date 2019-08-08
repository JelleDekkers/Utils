using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class SerializedClassDrawerAttribute : PropertyAttribute
{
    public Type TargetType;
    public Type TargetAttributeType;
}