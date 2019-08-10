using System.Collections.Generic;
using UnityEngine;
using Utils.Core.Flow;

public class TestAction : StateAction
{
    public override string DisplayName => "This is a test";

#pragma warning disable CS0649
    [SerializeField] private float testValue;
#pragma warning restore CS0649
    [SerializeField] private GameObject testGameObject;
    [SerializeField] private List<TestChild> testList;

    public override void OnEnter()
    {
        Debug.Log("OnStateStart() TestAction, value: " + testValue);
    }

    public override void OnExit()
    {
        Debug.Log("OnStateExit() TestAction");
    }
}

[System.Serializable]
public class TestChild
{
    public float value;
}
