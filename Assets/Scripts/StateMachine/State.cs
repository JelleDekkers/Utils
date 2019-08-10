using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachineData"/>
    /// </summary>
    [Serializable]
    public class State : ScriptableObject
    {
        public List<StateAction> TemplateActions = new List<StateAction>();
        public List<StateAction> RunTimeActions;

        public List<RuleGroup> RuleGroups = new List<RuleGroup>();

        public string Title = "New State";
        public Vector2 Position;

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }

        public void Start()
        {
            RunTimeActions = new List<StateAction>();
            for (int i = 0; i < TemplateActions.Count; i++)
            {
                RunTimeActions.Add(Instantiate(TemplateActions[i]));
            }
        }

        public void Stop()
        {
            RunTimeActions = null;
        }
    }
}