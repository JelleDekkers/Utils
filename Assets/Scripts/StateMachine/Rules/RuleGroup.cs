﻿using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class RuleGroup : ScriptableObject
    {
        public State Destination => destination;
        [SerializeField] private State destination;

        public List<Rule> Rules => rules;
        [SerializeField] private List<Rule> rules = new List<Rule>();

#if UNITY_EDITOR
        public BezierLine line = new BezierLine();
#endif

        public bool AllRulesAreValid()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                if(!rules[i].IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetDestination(State state)
        {
            destination = state;
        }

        public void AddRule(Rule rule)
        {
            rules.Add(rule);
        }

        public void RemoveRule(Rule rule)
        {
            rules.Remove(rule);
        }
    }
}