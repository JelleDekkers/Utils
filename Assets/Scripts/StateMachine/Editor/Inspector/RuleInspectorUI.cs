using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class RuleInspectorUI : InspectorUI
    {
        private Rule Rule { get { return InspectedProperty.serializedObject.targetObject as Rule; } }

        protected override void DrawHeader()
        {
            base.DrawHeader();
        }
    }
}