using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling multiple rules, when all <see cref="RuntimeRules"/> are valid, the stateMachine can transition to <see cref="Destination"/>
    /// </summary>
    [System.Serializable]
    public class RuleGroup
    {
        public List<Rule> TemplateRules = new List<Rule>();
        public List<Rule> RuntimeRules;

        public State Destination;
        public LinkData linkData = new LinkData();

        public bool AllRulesAreValid()
        {
            for (int i = 0; i < RuntimeRules.Count; i++)
            {
                if (!RuntimeRules[i].IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetDestination(State state)
        {
            Destination = state;
        }

        public void OnActivate()
        {
            RuntimeRules = new List<Rule>();
            for (int i = 0; i < TemplateRules.Count; i++)
            {
                RuntimeRules.Add(Object.Instantiate(TemplateRules[i]));
            }
        }

        public void OnDeactivatie()
        {
            RuntimeRules = null;
        }
    }
}