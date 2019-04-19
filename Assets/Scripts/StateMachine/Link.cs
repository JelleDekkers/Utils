using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for linking a <see cref="Source"/> and <see cref="Destination"/> of type <see cref="ILinkConnectionPoint"/> 
    /// </summary>
    public class Link : ScriptableObject
    {
        public LinkConnection Source => source;
        [SerializeField] private LinkConnection source;

        public LinkConnection Destination => destination;
        [SerializeField] private LinkConnection destination;

        public Link(LinkConnection source, LinkConnection destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void SetSource(LinkConnection source)
        {
            this.source = source;
        }

        public void SetDestination(LinkConnection destination)
        {
            this.destination = destination;
        }
    }

    [System.Serializable]
    public class LinkConnection
    {
        public enum ConnectionType
        {
            In,
            Out
        }

        public ConnectionType connectionType;

        public LinkConnection(ConnectionType connectionType)
        {
            this.connectionType = connectionType;
        }
    }
}