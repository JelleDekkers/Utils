using System;
using UnityEngine;
using UnityEditor;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering links between 2 <see cref=" StateRenderer"/>s, uses <see cref="LinkData"/> for serialization
    /// </summary>
    [Serializable]
    public class LinkRenderer
    {
#if UNITY_EDITOR
        private const float ARROW_HEIGHT = 20f;
        private const float ARROW_WIDTH = 30f;
        private readonly Color ArrowColor = new Color(0.6f, 0, 0);

        private Handle sourceHandle;
        private Handle destinationHandle;

        private LinkData data;
        private Texture2D arrowTexture;

        public LinkRenderer(LinkData data)
        {
            this.data = data;
            sourceHandle = new Handle(data.sourceHandleData);
            destinationHandle = new Handle(data.destinationHandleData);
        }

        public void Draw(Vector2 source, Vector2 destination, Color color, float thickness, bool showHandles = false)
        {
            Handles.BeginGUI();
            sourceHandle.Update(source);
            destinationHandle.Update(destination);

            if (showHandles)
            {
                sourceHandle.Draw();
                destinationHandle.Draw();
            }

            DrawLine(source, destination, color, thickness);
            DrawArrow(source, destination, sourceHandle.Rect.position, destinationHandle.Rect.position, color);

            Handles.EndGUI();
        }

        public void ProcessEvents(Event e)
        {
            sourceHandle.ProcessEvents(e);
            destinationHandle.ProcessEvents(e);
        }

        public void Reset()
        {
            data.Reset();
        }

        private void DrawLine(Vector2 source, Vector2 destination, Color color, float thickness)
        {
            Handles.DrawBezier(
                source,
                destination,
                sourceHandle.Rect.position,
                destinationHandle.Rect.position,
                color,
                null,
                thickness
            );
        }

        private void DrawArrow(Vector2 start, Vector2 end, Vector2 source, Vector2 destination, Color arrowColor)
        {
            Vector2 pointA = Handles.MakeBezierPoints(start, end, source, destination, 12)[7];
            Vector2 pointB = Handles.MakeBezierPoints(start, end, source, destination, 12)[6];

            float angle = Mathf.Round(Math.SignedAngle(Vector2.right, pointA - pointB, Vector3.forward));
            float height = ARROW_HEIGHT * .5f;
            float width = ARROW_WIDTH * .5f;

            if(arrowTexture == null)
            {
                arrowTexture = DrawHelper.CreateArrowTexture(arrowColor);
            }

            GUIUtility.RotateAroundPivot(angle, pointB);
            GUI.DrawTexture(new Rect(pointB.x - width * .5f, pointB.y - height * .5f, width, height), arrowTexture);
            GUI.matrix = Matrix4x4.identity;
        }

        /// <summary>
        /// Class for drawing and handling <see cref="LinkRenderer"/> curve
        /// </summary>
        [Serializable]
        public class Handle : IDraggable
        {
            public Rect Rect { get { return new Rect(originPoint + data.offset - Size / 2, Size); } }

            private const float LINE_THICKNESS = 3f;
            private readonly Vector2 Size = new Vector2(10, 10);
            private readonly Color knobColor = Color.grey;

            private Vector2 originPoint;
            private bool isDragged;
            private LinkData.HandleData data;

            public Handle(LinkData.HandleData data)
            {
                this.data = data;
            }

            public void Update(Vector2 originPoint)
            {
                this.originPoint = originPoint;
            }

            public void Draw()
            {
                Handles.DrawBezier(
                    originPoint,
                    Rect.position + Size / 2,
                    originPoint,
                    Rect.position + Size / 2,
                    knobColor,
                    null,
                    LINE_THICKNESS
                );

                Color previousColor = GUI.color;
                GUI.color = knobColor;
                GUI.Box(Rect, "", GUIStyles.BezierLineHandleStyle);
                GUI.color = previousColor;
            }

            public void ProcessEvents(Event e)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (e.button == 0 && Rect.Contains(e.mousePosition))
                        { 
                            OnDragStart(e);
                            e.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if(isDragged)
                        {
                            OnDragEnd(e);
                        }
                        break;
                    case EventType.MouseDrag:
                        if (e.button == 0 && isDragged)
                        {
                            OnDrag(e);
                            e.Use();
                        }
                        break;
                }
            }

            public void OnDragStart(Event e)
            {
                isDragged = true;
            }

            public void OnDrag(Event e)
            {
                data.offset += e.delta;
                GUI.changed = true;
            }

            public void OnDragEnd(Event e)
            {
                isDragged = false;
            }
        }
#endif
    }
}