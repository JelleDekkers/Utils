using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="global::StateMachine.RuleGroup"/>s on <see cref="StateMachineEditorManager"/>
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
        private StateMachineEditorManager manager;
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
       
        public RuleGroupRenderer(RuleGroup ruleGroup, StateRenderer state, StateMachineEditorManager stateMachine)
        {
            RuleGroup = ruleGroup;
            stateRenderer = state;
            manager = stateMachine;

            Rect = new Rect();
        }

        public void ProcessEvents(Event e)
        {
            if (isDraggingLine)
            {
                OnDrag(e);
            }

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (IsSelected && e.keyCode == (KeyCode.Delete))
                    {
                        DeleteRuleGroup();
                        e.Use();
                    }

                    if (isDraggingLine && e.keyCode == KeyCode.Escape)
                    {
                        OnDragEnd(e);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDraggingLine)
                    {
                        OnDragEnd(e);
                        e.Use();
                    }
                    break;

                case EventType.MouseDown:
                    if (e.button == 0) 
                    {
                        if (IsSelected && !Rect.Contains(e.mousePosition))
                        {
                            OnDeselect(e);
                        }
                        else if (Rect.Contains(e.mousePosition))
                        {
                            OnSelect(e);
                        }
                    }
                    else if(e.button == 1)
                    {
                        if(IsSelected && Rect.Contains(e.mousePosition))
                        {
                            OnDragStart(e);
                            e.Use();
                        }
                    }
                    break;
            }
        }

        public void SetAsNew(Event e)
        {
            OnDragStart(e);
        }

        public void OnSelect(Event e)
        {
            IsSelected = true;
            manager.Select(this);
            GUI.changed = true;
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
            manager.Deselect(this);
            isDraggingLine = false;
            GUI.changed = true;
        }

        public void OnDragStart(Event e)
        {
            isDraggingLine = true;
        }

        public void OnDrag(Event e)
        {
            DrawLine(e.mousePosition);
            GUI.changed = true;
        }

        public void OnDragEnd(Event e)
        {
            if (manager.IsStateAtPosition(e.mousePosition, out StateRenderer stateRenderer))
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
            else
            {
                RuleGroup.SetDestination(null);
            }

            isDraggingLine = false;
            GUI.changed = true;
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
                    GUI.Label(ruleRect, RuleGroup.Rules[i].DisplayName + " " + IsSelected.ToString() + " " + Rect.Contains(Event.current.mousePosition).ToString(), RuleStyle);

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

            manager.Deselect(this);
            GUI.changed = true;
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