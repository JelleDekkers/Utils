using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    public class RuleGroup : ScriptableObject
    {
        public State Destination => destination;
        [SerializeField] private State destination;

        public List<Rule> Rules { get { return rules; } set { rules = value; } }
        [SerializeField] private List<Rule> rules = new List<Rule>();

        public LinkData linkData = new LinkData();

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
    }
}