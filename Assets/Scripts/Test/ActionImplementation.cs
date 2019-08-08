using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionImplementation : StateAction
{
    public int testInt;
    public float testFloat;
    public string testString;
    public Vector3 testVector3;
    public List<int> testList;
    public ArrayTest[] testObject;

    public override void Test()
    {
        base.Test();
    }
}

[System.Serializable]
public class ArrayTest
{
    public float testvalue;
}

