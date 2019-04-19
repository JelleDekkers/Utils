using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering individual <see cref="global::StateMachine.Rule"/>s on <see cref="StateMachineRenderer"/>
    /// </summary>
    public class RuleRenderer
    {
        public const float RULE_HEIGHT = StateRenderer.HEADER_HEIGHT;
        public const float LINE_THICKNESS = 3f;

        public Rule Rule { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }
        
        private readonly Color HighlightSelectionColor = Color.blue;
        private readonly Color HighlightNoDestinationColor = Color.red;
        private readonly float HighlightMargin = 5;

        private Vector2 SourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }

        public RuleRenderer(Rule rule)
        {
            Rule = rule;
            Rect = new Rect();
        }

        public bool ProcessEvents(Event e)
        {
            bool guiChanged = false;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) 
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            OnSelect();
                        }
                        else
                        {
                            if (IsSelected)
                            {
                                OnDeselect();
                                guiChanged = true;
                            }
                        }
                    }
                    break;

                    // TODO: voor dragging new link destination
                //case EventType.MouseUp:
                //    if (isDragged)
                //    {
                //        OnDragEnd();
                //    }
                //    else if (drawingNewRuleLink)
                //    {
                //        StopDrawNewRuleLink(e.mousePosition);
                //    }
                //    break;

                //case EventType.MouseDrag:
                //    if (e.button == 0 && isDragged)
                //    {
                //        Drag(e.delta);
                //        e.Use();
                //        guiChanged = true;
                //    }
                //    break;
            }

            return guiChanged;
        }

        public void OnSelect()
        {
            // TODO: color change
            IsSelected = true;
            ShowInspector();
        }

        public void OnDeselect()
        {
            IsSelected = false;
        }

        public void OnDelete()
        {

        }

        private void ShowInspector()
        {
            // TODO: inspector show rule
        }

        public Rect Draw(Vector2 position, float width)
        {
            if (IsSelected)
            {
                DrawHighlight(HighlightSelectionColor);
            }
            else if (Rule.Destination == null)
            {
                DrawHighlight(HighlightNoDestinationColor);
            }

            Rect = new Rect(position.x, position.y, width, RULE_HEIGHT);
            GUI.Box(Rect, Rule.DisplayName);

            if (Rule.Destination != null)
            { 
                Vector2 destinationPoint = Rule.Destination.Position;
                DrawLine(SourcePoint, destinationPoint);
            }

            return Rect;
        }

        private void DrawHighlight(Color color)
        {
            Color previousColor = GUI.color;

            Rect r = new Rect(
                Rect.x - HighlightMargin / 2,
                Rect.y - HighlightMargin / 2,
                Rect.width + HighlightMargin,
                Rect.height + HighlightMargin);

            GUI.color = color;
            GUI.Box(r, "");

            GUI.color = previousColor;
        }

        public void DrawLine(Vector2 destination)
        {
            DrawLine(SourcePoint, destination);
        }

        private void DrawLine(Vector2 source, Vector2 destination)
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