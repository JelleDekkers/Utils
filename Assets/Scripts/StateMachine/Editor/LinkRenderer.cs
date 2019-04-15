using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class LinkRenderer
    {
        private const float LINE_THICKNESS = 3f;

        public Link Link { get; private set; }

        private Vector2 SourcePoint { get { return Link.Source.ConnectionPointPosition; } }
        private Vector2 DestinationPoint { get { return Link.Destination.ConnectionPointPosition; } }

        public LinkRenderer(Link rule)
        {
            Link = rule;
        }

        public void Draw()
        {
            Draw(SourcePoint, DestinationPoint);
        }

        public void Draw(Vector2 destination)
        {
            Draw(SourcePoint, destination);
        }

        public void Draw(Vector2 source, Vector2 destination)
        {
            Handles.BeginGUI();

            Handles.DrawBezier(
               source,
               destination,
               source - Vector2.left * 50f,
               destination + Vector2.left * 50f,
               Color.red,
               null,
               LINE_THICKNESS
           );

            Handles.EndGUI();
        }
    }
}