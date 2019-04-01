using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    [CreateAssetMenu(menuName = "Actions/Testing/Add Health")]
    public class ChangeHealth : StateActions
    {
        public override void Execute(StateManager stateManager)
        {
            stateManager.health += 10;
        }
    }
}