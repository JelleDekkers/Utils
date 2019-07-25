using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class TitleTestRule : Rule
    {
        public override string DisplayName => title;

        [SerializeField] private string title;
    }
}