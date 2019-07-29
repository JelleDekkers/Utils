using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class TitleTestRule : Rule
    {
        public override string DisplayName => title;

#pragma warning disable CS0649
        [SerializeField] private string title;
#pragma warning restore CS0649
    }
}