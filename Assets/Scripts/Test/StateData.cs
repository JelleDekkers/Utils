using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateData : ICloneable
{
    [GuidResource(typeof(GameObject))] public GameObject gObject;
    public string name = "New State";
    public List<StateActionData> actions = new List<StateActionData>();

    public object Clone()
    {
        return MemberwiseClone();
    }
}

public class GuidResourceAttribute : PropertyAttribute
{
    public Type BaseType { get; }

    public GuidResourceAttribute(Type baseType)
    {
        BaseType = baseType;
    }
}

