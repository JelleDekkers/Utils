using UnityEngine;

namespace Utils.Core.Flow
{
    public class EmptyRule : Rule
    {
        public override string DisplayName => "Empty Rule";

        public override bool IsValid => true;

        [SerializeField] private float testValue;
    }
}