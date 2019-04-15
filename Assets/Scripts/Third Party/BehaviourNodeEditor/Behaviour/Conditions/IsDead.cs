using UnityEngine;

namespace NodeEditor
{
    [CreateAssetMenu(menuName = "Conditions/Is Dead")]
    public class IsDead : Condition
    {
        public override bool CheckCondition(StateManager manager)
        {
            return manager.health <= 0;
        }
    }
}