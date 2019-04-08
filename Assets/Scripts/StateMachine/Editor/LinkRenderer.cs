using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class LinkRenderer
    {
        private const float LINE_THICKNESS = 3f;

    //    public Link Link { get; private set; }

    //    private State Source;// { get { return Link.Source; } }
    //    private State Destination { get { return Link.Destination; } }

    //    public LinkRenderer(Link rule)
    //    {
    //        Link = rule;
    //    }

    //    public void Draw()
    //    {
    //        if(Source == null && Destination == null) { return; }

    //        //Draw(Destination.Position);
    //    }

        public void Draw(Vector2 source, Vector2 destination)
        {
            Vector2 clampedSource = source;
            Vector2 clampedDestination = destination;

            // clampen tussen canvasWindow position en size
            //if (destination.x < canvasWindow.width)
            //    clampedDestination.x = 0;
            //if (destination.y > canvasWindow.height + canvasWindow.y)
            //    clampedDestination.y = canvasWindow.height + canvasWindow.y;

            Handles.BeginGUI();

            Handles.DrawBezier(
               clampedSource,
               clampedDestination,
               clampedSource - Vector2.left * 50f,
               clampedDestination + Vector2.left * 50f,
               Color.red,
               null,
               LINE_THICKNESS
           );

            //if (Handles.Button((EntryPoint.Position + ExitPoint.Position) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleCap))
            //{

            //}

            Handles.EndGUI();
        }
    }
}