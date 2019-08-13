using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachineData"/>
    /// </summary>
    [Serializable]
    public class State : ScriptableObject, INode
    {
        public List<StateAction> TemplateActions = new List<StateAction>();
        public List<StateAction> RunTimeActions;

        public List<RuleGroup> RuleGroups = new List<RuleGroup>();

        public string Title = "New State";

        [SerializeField] private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }

        public void OnStart()
        {
            RunTimeActions = new List<StateAction>();
            for (int i = 0; i < TemplateActions.Count; i++)
            {
                RunTimeActions.Add(Instantiate(TemplateActions[i]));
            }
        }

        public void OnExit()
        {
            RunTimeActions = null;
        }
    }
}