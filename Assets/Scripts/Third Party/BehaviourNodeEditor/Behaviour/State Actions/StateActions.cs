using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    public abstract class StateActions : ScriptableObject
    {
        public abstract void Execute(StateManager stateManager);
    }
}