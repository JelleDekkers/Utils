using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.RuleGroup"/>s on <see cref="StateMachineRenderer"/>
    /// </summary>
    public class RuleGroupRenderer : ISelectable, IDraggable, IInspectable
    {
        private const float RULE_HEIGHT = StateRenderer.HEADER_HEIGHT;
        private const float LINE_THICKNESS = 3f;
        private const float HIGHLIGHT_MARGIN_SIZE = 5;

        public RuleGroup RuleGroup { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }
        public ScriptableObject InspectableObject => RuleGroup;

        private readonly Color HighlightSelectionColor = Color.blue;
        private readonly Color HighlightNoDestinationColor = Color.red;

        private Vector2 SourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }
        
        private StateRenderer stateRenderer;
        private StateMachineRenderer stateMachineRenderer;
        private bool isDraggingLine;

        // TODO: move into seperate Style script
        private GUIStyle RuleStyle
        {
            get
            {
                if(ruleStyle == null)
                {
                    ruleStyle = new GUIStyle();
                    ruleStyle.alignment = TextAnchor.MiddleRight;
                    ruleStyle.padding.right = 3;
                    ruleStyle.padding.left = 3;
                    ruleStyle.padding.top = 3;
                    ruleStyle.padding.bottom = 3;
                }
                return ruleStyle;
            }
        }
        private GUIStyle ruleStyle;
       
        public RuleGroupRenderer(RuleGroup ruleGroup, StateRenderer state, StateMachineRenderer stateMachine)
        {
            RuleGroup = ruleGroup;
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
                    if (IsSelected && e.keyCode == (KeyCode.Delete))
                    {
                        DeleteRuleGroup();
                        e.Use();
                        guiChanged = true;
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

        public void SetAsNew()
        {
            isDraggingLine = true;
        }

        public void OnSelect(Event e)
        {
            IsSelected = true;
            stateMachineRenderer.Select(this);
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
            stateMachineRenderer.Deselect(this);
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
                    RuleGroup.SetDestination(null);
                }
                else
                {
                    RuleGroup.SetDestination(stateRenderer.State);
                }
            }
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, DeleteRuleGroup);
            menu.ShowAsContext();

            e.Use();
        }

        public Rect Draw(Vector2 position, float width)
        {
            if (IsSelected)
            {
                DrawHighlight(HighlightSelectionColor);
            }
            else if (RuleGroup.Destination == null)
            {
                DrawHighlight(HighlightNoDestinationColor);
            }

            if (RuleGroup.Destination != null && !isDraggingLine)
            { 
                Vector2 destinationPoint = new Vector2(RuleGroup.Destination.Rect.x, RuleGroup.Destination.Rect.y + RuleGroup.Destination.Rect.height / 2);
                DrawLine(SourcePoint, destinationPoint);
            }

            DrawRules(position, width);

            return Rect;
        }

        private void DrawRules(Vector2 position, float width)
        {
            DrawHelper.DrawLinkNode(new Vector2(Rect.x + Rect.width, Rect.y + Rect.height / 2));

            if (RuleGroup.Rules.Count == 0)
            {
                Rect = new Rect(position.x, position.y, width, RULE_HEIGHT);
                GUI.Box(Rect, "TRUE");
            }
            else
            {
                float yPos = position.y;
                float totalHeight = 0;
                for (int i = 0; i < RuleGroup.Rules.Count; i++)
                {
                    yPos += RULE_HEIGHT;
                    totalHeight += RULE_HEIGHT;
                }
                Rect = new Rect(position.x, position.y, width, totalHeight);
                GUI.Box(Rect, "");

                yPos = position.y;
                for (int i = 0; i < RuleGroup.Rules.Count; i++)
                {
                    Rect ruleRect = new Rect(position.x, yPos, width, RULE_HEIGHT);
                    GUI.Label(ruleRect, RuleGroup.Rules[i].DisplayName, RuleStyle);

                    yPos += RULE_HEIGHT;
                    totalHeight += RULE_HEIGHT;
                }
            }

        }

        private void DrawHighlight(Color color)
        {
            Color previousColor = GUI.color;

            Rect r = new Rect(
                Rect.x - HIGHLIGHT_MARGIN_SIZE / 2,
                Rect.y - HIGHLIGHT_MARGIN_SIZE / 2,
                Rect.width + HIGHLIGHT_MARGIN_SIZE,
                Rect.height + HIGHLIGHT_MARGIN_SIZE);

            GUI.color = color;
            GUI.Box(r, "");

            GUI.color = previousColor;
        }

        private void DeleteRuleGroup()
        {
            stateRenderer.State.RemoveRuleGroup(RuleGroup);
            stateRenderer.InitializeRuleRenderers();

            stateMachineRenderer.Deselect(this);
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