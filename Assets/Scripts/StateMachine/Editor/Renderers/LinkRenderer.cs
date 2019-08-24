using System;
using UnityEngine;
using UnityEditor;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for rendering links between 2 <see cref=" StateRenderer"/>s, uses <see cref="LinkData"/> for serialization
    /// </summary>
    public class LinkRenderer
    {
        private const float ARROW_HEIGHT = 20f;
        private const float ARROW_WIDTH = 30f;
        private readonly Color ArrowColor = Color.red;
        private readonly KeyCode showHandlesKey = KeyCode.LeftAlt;

        private Handle sourceHandle;
        private Handle destinationHandle;
        private LinkData data;
        private Texture2D arrowTexture;
        private Texture2D selectedArrowTexture;
        private bool showHandles;
        private float lineThickness;

        public LinkRenderer(LinkData data)
        {
            this.data = data;
            sourceHandle = new Handle(data.sourceHandleData);
            destinationHandle = new Handle(data.destinationHandleData);

            arrowTexture = DrawHelper.CreateArrowTexture(ArrowColor);
            selectedArrowTexture = DrawHelper.CreateArrowTexture(NodeGUIStyles.LINK_COLOR_SELECTED);
        }

        public void Draw(Vector2 source, Vector2 destination, Color color, float thickness, bool isSelected)
        {
            lineThickness = thickness;

            Handles.BeginGUI();
            sourceHandle.Update(source);
            destinationHandle.Update(destination);

            DrawLine(source, destination, color, thickness);

            if (showHandles)
            {
                sourceHandle.Draw();
                destinationHandle.Draw();
            }

            if (isSelected)
            {
                DrawArrow(selectedArrowTexture, source, destination, sourceHandle.Rect.position, destinationHandle.Rect.position);
            }
            else
            {
                showHandles = false;
                DrawArrow(arrowTexture, source, destination, sourceHandle.Rect.position, destinationHandle.Rect.position);
            }
            Handles.EndGUI();
        }

        public void ProcessEvents(Event e)
        {
            if (e.keyCode == showHandlesKey)
            {
                if (e.type == EventType.KeyDown)
                {
                    showHandles = true;
                    GUI.changed = true;
                }
                else if(e.type == EventType.KeyUp)
                {
                    showHandles = false;
                    GUI.changed = true;
                }
            }

            sourceHandle.ProcessEvents(e);
            destinationHandle.ProcessEvents(e);
        }

        public void Reset()
        {
            data.Reset();
        }

        public bool IsHovering(Vector2 mousePosition)
        {
            float dist = HandleUtility.DistancePointBezier(mousePosition, sourceHandle.OriginPoint, destinationHandle.OriginPoint, sourceHandle.Rect.position, destinationHandle.Rect.position);
            return dist < lineThickness;
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

        private void DrawArrow(Texture2D arrow, Vector2 start, Vector2 end, Vector2 source, Vector2 destination)
        {
            Vector2 pointA = Handles.MakeBezierPoints(start, end, source, destination, 12)[7];
            Vector2 pointB = Handles.MakeBezierPoints(start, end, source, destination, 12)[6];

            float angle = Mathf.Round(Math.SignedAngle(Vector2.right, pointA - pointB, Vector3.forward));
            float height = ARROW_HEIGHT * .5f;
            float width = ARROW_WIDTH * .5f;

            GUIUtility.RotateAroundPivot(angle, pointB);
            GUI.DrawTexture(new Rect(pointB.x - width * .5f, pointB.y - height * .5f, width, height), arrow);
            GUI.matrix = Matrix4x4.identity;
        }

        /// <summary>
        /// Class for drawing and handling <see cref="LinkRenderer"/> curve
        /// </summary>
        [Serializable]
        public class Handle : IDraggable
        {
            public Rect Rect { get { return new Rect(OriginPoint + data.offset - Size / 2, Size); } }

            private const float LINE_THICKNESS = 3f;
            private readonly Vector2 Size = new Vector2(10, 10);
            private readonly Color knobColor = Color.grey;

            public Vector2 OriginPoint { get; private set; }
            public bool IsDragged { get; private set; }

            private LinkData.HandleData data;

            public Handle(LinkData.HandleData data)
            {
                this.data = data;
            }

            public void Update(Vector2 originPoint)
            {
                OriginPoint = originPoint;
            }

            public void Draw()
            {
                Handles.DrawBezier(
                    OriginPoint,
                    Rect.position + Size / 2,
                    OriginPoint,
                    Rect.position + Size / 2,
                    knobColor,
                    null,
                    LINE_THICKNESS
                );

                Color previousColor = GUI.color;
                GUI.color = knobColor;
                GUI.Box(Rect, "", NodeGUIStyles.BezierLineHandleStyle);
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
                        if(IsDragged)
                        {
                            OnDragEnd(e);
                        }
                        break;
                    case EventType.MouseDrag:
                        if (e.button == 0 && IsDragged)
                        {
                            OnDrag(e);
                            e.Use();
                        }
                        break;
                }
            }

            public void OnDragStart(Event e)
            {
                IsDragged = true;
            }

            public void OnDrag(Event e)
            {
                data.offset += e.delta;
                GUI.changed = true;
            }

            public void OnDragEnd(Event e)
            {
                IsDragged = false;
            }
        }
    }
}