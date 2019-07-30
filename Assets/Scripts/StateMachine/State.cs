﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Abstract class for states, used in <see cref="StateMachineData"/>
    /// </summary>
    [Serializable]
    public class State : ScriptableObject, ICloneable
    {
        public List<StateAction> Actions { get { return actions; } set { actions = value; } }
        [SerializeField] private List<StateAction> actions = new List<StateAction>();

        public List<RuleGroup> RuleGroups { get { return ruleGroups; } set { ruleGroups = value; } }
        [SerializeField] private List<RuleGroup> ruleGroups = new List<RuleGroup>();

        public string Title = "New State";
        public Vector2 Position;        

        public override string ToString()
        {
            return string.Format("State '{0}'", Title);
        }

        public object Clone()
        {
            return (State)MemberwiseClone();
        }
    }
}