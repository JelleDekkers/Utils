using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class TestGameObject : MonoBehaviour
{
    //[InspectableSO]
    //public TestBase test;
    //[InspectableSO]
    //public List<TestBase> testList = new List<TestBase>();

    [HideInInspector] public StateAction action;
    public Container container;

    public void AddToTest(StateAction test)
    {
        container.actions.Add(test);
    }

    public void SetTest(StateAction test)
    {
        action = test;
    }
}

[System.Serializable]
public class Container
{
    public List<StateAction> actions = new List<StateAction>();

}
