using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachine"/>
    /// </summary>
    [Serializable]
    public class State
    {
        public List<StateAction> Actions => actions;
        [SerializeField] private List<StateAction> actions = new List<StateAction>();

        public List<Rule> Rules => rules;
        [SerializeField] private List<Rule> rules = new List<Rule>();

#if UNITY_EDITOR
        public string Title /*{ get; set; }*/ = "New State";
        public Vector2 Position; /*{ get; set; }*/
#endif

        public void AddAction(StateAction action)
        {
            actions.Add(action);
        }

        public void RemoveAction(StateAction action)
        {
            actions.Remove(action);
            UnityEngine.Object.DestroyImmediate(action);
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