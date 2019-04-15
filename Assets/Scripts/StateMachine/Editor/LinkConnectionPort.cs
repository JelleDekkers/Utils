using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public enum ConnectionType
    {
        In,
        Out
    }

    public class LinkConnectionPort
    {
        public static Vector2 SIZE = new Vector2(10, 10);

        private readonly Color BackgroundColor = new Color(0.5f, 0.5f, 1f);

        private Rect rect;
        private ConnectionType connectionType;
        private bool isDragging;

        public LinkConnectionPort(ConnectionType connectionType)
        {
            this.connectionType = connectionType;

            rect = new Rect
            {
                size = SIZE
            };
        }
        
        public void Draw(Vector2 position)
        {
            rect.position = position;

            GUI.color = BackgroundColor;
            GUI.Box(rect, ">");
        }

        // TODO: needs to call gui.changed? i.e. return value
        public void ProcessEvents(Event e)
        {
            // if connectionType == IN

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            OnDragStart();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    OnDragEnd();
                    break;

                case EventType.MouseDrag:
                    
                    break;
            }
            if (isDragging)
            {
                DrawTempLine(e);
                //e.Use();
                //return true;
            }
            //return false;
        }

        private void OnDragStart()
        {
            isDragging = true;
        }

        private void OnDragEnd()
        {
            isDragging = false;
        }

        private void DrawTempLine(Event e)
        {
            Vector2 destinationPosition = e.mousePosition;
            float bezierStrength = 50f;

            Handles.BeginGUI();
            Handles.DrawBezier(
               rect.position,
               destinationPosition,
               new Vector2(100, 100),
               destinationPosition + Vector2.left * bezierStrength,
               Color.red,
               null,
               3f
           );
            Handles.EndGUI();
            HandleUtility.Repaint();
        }
    }
}