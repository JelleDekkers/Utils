using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachine"/>
    /// </summary>
    [Serializable]
    public class State : ScriptableObject
    {
        public List<StateAction> Actions => actions;
        [SerializeField] private List<StateAction> actions = new List<StateAction>();

        public List<Rule> Rules => rules;
        [SerializeField] private List<Rule> rules = new List<Rule>();

        // Editor stuff:
        public string Title = "New State";
        public Vector2 Position;

        public void AddAction(StateAction action)
        {
            actions.Add(action);
        }

        public void RemoveAction(StateAction action)
        {
            actions.Remove(action);
        }

        public void AddRule(Rule rule)
        {
            rules.Add(rule);
        }

        public void RemoveRule(Rule rule)
        {
            rules.Remove(rule);
        }

        public void Run()
        {
            foreach(StateAction action in actions)
            {
                action.Start();
            }
        }
    }
}