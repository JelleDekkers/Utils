using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Test
{
    public int value;
}

public class StateMachineDataTest : ScriptableObject, ISerializationCallbackReceiver
{
    // moet uitendelijk stateData zijn en die heeft dan weer een list met StateActionData
    //public List<StateActionData> serializedActions = new List<StateActionData>();
    //public List<StateAction> actions = new List<StateAction>();

    //[SerializeField] public StateActionData serializedAction;
    //[SerializeField] public StateAction action;

    public State state;
    public StateData serializedState;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        //serializedActions.Clear();

        //for (int i = 0; i < actions.Count; i++)
        //{
        //    serializedActions.Add(new StateActionData(actions[i]));
        //}

        if (state != null)
        {
            // clone is misschien niet nodig?
            serializedState = state.data;
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        //actions.Clear();

        //for (int i = 0; i < serializedActions.Count; i++)
        //{
        //    actions.Add(serializedActions[i].Instantiate());
        //}

        if (serializedState != null)
        {
            state = new State(serializedState);
        }
    }
}