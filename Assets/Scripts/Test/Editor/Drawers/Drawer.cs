using System;
using UnityEngine;

public class Drawer
{
    public Type TargetType { get; }
    public Type TargetAttributeType { get; }

    private readonly ISerializedClassDrawer drawer;

    public Drawer(Type targetType, Type targetAttributeType, ISerializedClassDrawer drawer)
    {
        TargetType = targetType;
        TargetAttributeType = targetAttributeType;

        this.drawer = drawer;
    }

    public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
    {
        return drawer.Draw(label, value, type, attribute);
    }
}

