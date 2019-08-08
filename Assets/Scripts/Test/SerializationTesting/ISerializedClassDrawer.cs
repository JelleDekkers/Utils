using System;
using UnityEngine;

public interface ISerializedClassDrawer
{
    object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute);
}