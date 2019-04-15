using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for linking a <see cref="Source"/> and <see cref="Destination"/> of type <see cref="ILinkConnectionPoint"/> 
    /// </summary>
    public class Link
    {
        public ILinkConnectionPoint Source => source;
        [SerializeField] private ILinkConnectionPoint source;

        public ILinkConnectionPoint Destination => destination;
        [SerializeField] private ILinkConnectionPoint destination;

        public Link(ILinkConnectionPoint source, ILinkConnectionPoint destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void SetSource(ILinkConnectionPoint source)
        {
            this.source = source;
        }

        public void SetDestination(ILinkConnectionPoint destination)
        {
            this.destination = destination;
        }
    }
}