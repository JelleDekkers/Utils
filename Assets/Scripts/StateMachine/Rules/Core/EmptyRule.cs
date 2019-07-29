using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class EmptyRule : Rule
    {
        public override string DisplayName => "Empty Rule";

        public override bool IsValid => true;

        [SerializeField] private float testValue;
    }
}