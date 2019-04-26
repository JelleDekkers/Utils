using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class EmptyRule : Rule
    {
        public override string DisplayName => "TRUE";

        public override bool IsValid => true;

        [SerializeField] private float testValue;
    }
}