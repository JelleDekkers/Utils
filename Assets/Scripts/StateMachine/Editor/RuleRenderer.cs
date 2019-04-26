using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering individual <see cref="global::StateMachine.Rule"/>s on <see cref="StateMachineRenderer"/>
    /// </summary>
    public class RuleRenderer : ISelectable, IDraggable, IInspectable
    {
        public const float RULE_HEIGHT = StateRenderer.HEADER_HEIGHT;
        public const float LINE_THICKNESS = 3f;

        public Rule Rule { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }

        public string PropertyFieldName => "Rules";
        public ScriptableObject InspectableObject => Rule;
        public Type InspectorBehaviour => typeof(RuleInspectorUI);

        private readonly Color HighlightSelectionColor = Color.blue;
        private readonly Color HighlightNoDestinationColor = Color.red;
        private readonly float HighlightMargin = 5;

        private Vector2 SourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }


        private StateRenderer stateRenderer;
        private StateMachineRenderer stateMachineRenderer;
        private bool isDraggingLine;
        private bool isNew;

        public RuleRenderer(Rule rule, StateRenderer state, StateMachineRenderer stateMachine)
        {
            Rule = rule;
            stateRenderer = state;
            stateMachineRenderer = stateMachine;

            Rect = new Rect();
        }

        public bool ProcessEvents(Event e)
        {
            bool guiChanged = false;

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == (KeyCode.Delete))
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            stateRenderer.RemoveRule(Rule);
                        }
                    }
                    break;

                case EventType.MouseDown:
                    if (e.button == 0) 
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            OnSelect(e);
                        }
                        else
                        {
                            if (IsSelected)
                            {
                                OnDeselect(e);
                                guiChanged = true;
                            }
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 1 && Rect.Contains(e.mousePosition))
                    {
                        OnDrag(e);
                        e.Use();
                        guiChanged = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (isDraggingLine && e.button == 0 || e.button == 1)
                    {
                        OnDragEnd(e);
                        e.Use();
                        guiChanged = true;
                    }
                    break;
            }

            if(isDraggingLine)
            {
                OnDrag(e);
                guiChanged = true;

                if(e.keyCode == KeyCode.Escape)
                {
                    OnDragEnd(e);
                }
            }

            return guiChanged;
        }

        public void IsNew()
        {
            isDraggingLine = true;
        }

        public void OnSelect(Event e)
        {
            IsSelected = true;
            stateMachineRenderer.Inspector.Inspect(Rule);
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
        }

        public void OnDragStart(Event e) { }

        public void OnDrag(Event e)
        {
            DrawLine(e.mousePosition);
            isDraggingLine = true;
        }

        public void OnDragEnd(Event e)
        {
            // if hovering state, set rule destination
            // else set to null

            isDraggingLine = false;

            if (stateMachineRenderer.IsStateAtPosition(e.mousePosition, out StateRenderer stateRenderer))
            {
                if (stateRenderer.State == this.stateRenderer.State)
                {
                    Rule.SetDestination(null);
                }
                else
                {
                    Rule.SetDestination(stateRenderer.State);
                }
            }
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => stateRenderer.RemoveRule(this));
            menu.ShowAsContext();

            e.Use();
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

            if (Rule.Destination != null && !isDraggingLine)
            { 
                Vector2 destinationPoint = new Vector2(Rule.Destination.Rect.x, Rule.Destination.Rect.y + Rule.Destination.Rect.height / 2);
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

        private void DrawLine(Vector2 destination)
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