using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for handling multiple rules, when all <see cref="Rules"/> are valid, the stateMachine can transition to <see cref="Destination"/>
    /// </summary>
    [System.Serializable]
    public class RuleGroup
    {
        public List<Rule> Rules = new List<Rule>();

        public int Destination = -1;
        public LinkData linkData = new LinkData();

        public void Initialize()
        {
            List<Rule> copiedRules = Rules;
            for (int i = 0; i < Rules.Count; i++)
            {
                Rules[i] = Object.Instantiate(copiedRules[i]);
            }
        }

        public bool AllRulesAreValid()
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                if (!Rules[i].IsValid)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetDestination(State state)
        {
            Destination = state.ID;
        }

        public void OnActivate()
        {

        }

        public void OnDeactivatie()
        {

        }
    }
}