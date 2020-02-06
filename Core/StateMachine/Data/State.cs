using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for state nodes, used in <see cref="StateMachineScriptableObjectData"/>
    /// </summary>
    [Serializable]
    public class State : INode
    {
        private const string DEFAULT_NAME = "New State";

        public string Title;
        public bool IsActive { get; private set; }

        [SerializeField] private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        [SerializeField] private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public List<StateAction> TemplateActions = new List<StateAction>();
        public List<StateAction> RuntimeActions { get; private set; }

        public List<RuleGroup> RuleGroups = new List<RuleGroup>();

        public State(string name = DEFAULT_NAME)
        {
            id = GetHashCode();
            Title = name;
        }

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }

        public void OnStart()
        {
            CreateRuntimeStateActions();
            IsActive = true;
        }

        public void OnExit()
        {
            RuntimeActions = null;
            IsActive = false;
        }

        private void CreateRuntimeStateActions()
        {
            RuntimeActions = new List<StateAction>(TemplateActions.Count);
            for (int i = 0; i < TemplateActions.Count; i++)
            {
                RuntimeActions.Add(UnityEngine.Object.Instantiate(TemplateActions[i]));
            }
        }
    }
}