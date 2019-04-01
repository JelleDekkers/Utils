using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class TestAction : StateAction
{
    public override string DisplayName => "This is a test";

    [SerializeField] private float testValue;
    [SerializeField] private bool testBool;
    [SerializeField] private GameObject testGameObject;
    [SerializeField] private List<TestChild> testList;

    public override void Start()
    {
        Debug.Log("OnStateStart() TestAction, value: " + testValue);
    }

    public override void Stop()
    {
        Debug.Log("OnStateExit() TestAction");
    }
}

[System.Serializable]
public class TestChild
{
    public float value;
}
