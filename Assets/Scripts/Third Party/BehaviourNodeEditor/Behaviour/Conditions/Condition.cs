using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool CheckCondition(StateManager manager);
    }
}