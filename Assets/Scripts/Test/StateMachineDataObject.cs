using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachineDataObject : ScriptableObject
{
    public List<StateData> states;
    public List<StateAction> actions = new List<StateAction>()
    {
        new Action2Imp(),
        new ActionImplementation()
    };

}
