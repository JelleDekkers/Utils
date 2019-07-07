using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StateMachine
{
    public class BezierLine
    {
#if UNITY_EDITOR
        private LineHandle sourceHandle = new LineHandle(120);
        private LineHandle destinationHandle = new LineHandle(220);

        public void Draw(Vector2 source, Vector2 destination, Color color, float thickness)
        {
            Handles.BeginGUI();
            DrawLine(source, destination, color, thickness);
            sourceHandle.Draw(source);
            Handles.EndGUI();
        }

        public void ProcessEvents(Event e)
        {
            sourceHandle.ProcessEvents(e);
            destinationHandle.ProcessEvents(e);
        }

        private void DrawLine(Vector2 source, Vector2 destination, Color color, float thickness)
        {
            Handles.DrawBezier(
                source,
                destination,
                source - sourceHandle.tangent,
                destination + destinationHandle.tangent,
                color,
                null,
                thickness
            );
        }

        [System.Serializable]
        private class LineHandle
        {
            // TODO: berekenen a.d.h.v distance en angle
            public Vector2 tangent = Vector2.left * 50f;
            public float sourceHandleDistance = 10;
            public float sourceHandleAngle;
            private Rect rect;

            public LineHandle(float angle)
            {
                sourceHandleAngle = angle;
            }

            public void Draw(Vector2 position)
            {
                Color previousColor = GUI.color;

                // TODO: position berekenen a.d.h.v. position, distance en angle
            }

            public void ProcessEvents(Event e)
            {

            }

            private void Drag(Vector2 delta)
            {

            }
        }
#endif
    }
}