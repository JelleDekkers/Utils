using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling multiple rules, when all <see cref="Rules"/> are valid, the stateMachine can transition to <see cref="DestinationID"/>
    /// </summary>
    [System.Serializable]
    public class RuleGroup
    {
        public List<Rule> TemplateRules = new List<Rule>();
        public List<Rule> RuntimeRules { get; private set; }

        public int DestinationID = -1;
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
            DestinationID = state.ID;
        }

        public void OnActivate()
        {
            CreateRuntimeRules();
        }

        public void OnDeactivatie()
        {
            RuntimeRules = null;
        }

        private void CreateRuntimeRules()
        {
            RuntimeRules = new List<Rule>(TemplateRules.Count);
            for (int i = 0; i < TemplateRules.Count; i++)
            {
                RuntimeRules.Add(Object.Instantiate(TemplateRules[i]));
            }
        }
    }
}