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

        public List<StateAction> Actions = new List<StateAction>();
        public List<RuleGroup> RuleGroups = new List<RuleGroup>();

        public State(string name = DEFAULT_NAME)
        {
            id = GetHashCode();
            Title = name;
        }

        public void Initialize()
        {
            List<StateAction> copiedActions = Actions;
            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i] = UnityEngine.Object.Instantiate(copiedActions[i]);
            }

            foreach(RuleGroup group in RuleGroups)
            {
                group.Initialize();
            }
        }

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }

        public void OnStart()
        {
            IsActive = true;
        }

        public void OnExit()
        {
            IsActive = false;
        }
    }
}