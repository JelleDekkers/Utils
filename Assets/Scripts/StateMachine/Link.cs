using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class Link
    {
        public State Source => source;
        [SerializeField] private State source;

        public State Destination => destination;
        [SerializeField] private State destination;

        public Link(State source)
        {
            this.source = source;
        }

        public void SetSource(State source)
        {
            this.source = source;
        }

        public void SetDestination(State destination)
        {
            this.destination = destination;
        }
    }
}