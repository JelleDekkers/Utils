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
#if UNITY_EDITOR
        public string Title /*{ get; set; }*/ = "New State";
        public Vector2 Position; /*{ get; set; }*/
#endif

        public List<StateAction> Actions /*{ get; private set; }*/ = new List<StateAction>();

        public void AddAction(StateAction action)
        {
            Actions.Add(action);
        }

        public void RemoveAction(StateAction action)
        {
            Actions.Remove(action);
        }

        public void Run()
        {
            foreach(StateAction action in Actions)
            {
                action.Start();
            }
        }
    }
}