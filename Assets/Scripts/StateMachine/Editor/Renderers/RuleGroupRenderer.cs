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
        private const string EMPTY_RULE_DISPLAY_LABEL = "TRUE";

        public RuleGroup RuleGroup { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }
        public ScriptableObject InspectableObject => RuleGroup;

        private readonly Color HighlightNoDestinationColor = Color.red;
        private readonly Color RuleGroupDividerColor = Color.grey;

        private Vector2 SourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }
        
        private StateRenderer stateRenderer;
        private StateMachineEditorManager manager;
        private Rect fullRect;
        private bool isDraggingLine;
       
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
                        stateRenderer.RemoveRuleGroup(RuleGroup);
                        e.Use();
                        return;
                    }
                    else if (isDraggingLine && e.keyCode == KeyCode.Escape)
                    {
                        OnDragEnd(e);
                        e.Use();
                    }
                    break;

                case EventType.MouseDown:
                    if (e.button == 0) 
                    {
                        if (isDraggingLine)
                        {
                            OnDragEnd(e);
                            e.Use();
                        }

                        if (IsSelected && !Rect.Contains(e.mousePosition))
                        {
                            OnDeselect(e);
                        }
                        else if (Rect.Contains(e.mousePosition))
                        {
                            OnSelect(e);
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (IsSelected && Rect.Contains(e.mousePosition))
                        {
                            ShowContextMenu(e);
                        }
                    }
                    break;
            }
        }

        //public void SetAsNew(Event e)
        //{
        //    OnDragStart(e);
        //}

        #region Events
        public void OnSelect(Event e)
        {
            IsSelected = true;
            manager.Select(this);
            stateRenderer.SelectedRule = this;
            GUI.changed = true;
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
            manager.Deselect(this);
            isDraggingLine = false;

            if(stateRenderer.SelectedRule == this)
            {
                stateRenderer.SelectedRule = null;
            }

            GUI.changed = true;
        }

        public void OnDragStart(Event e)
        {
            isDraggingLine = true;
        }

        public void OnDrag(Event e)
        {
            DrawLine(e.mousePosition, GUIStyles.NODE_LINE_COLOR);
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
        #endregion

        #region Drawing
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete rulegroup"), false, () => stateRenderer.RemoveRuleGroup(RuleGroup));
            menu.AddItem(new GUIContent("Move up"), false, () => throw new NotImplementedException());
            menu.AddItem(new GUIContent("Move down"), false, () => throw new NotImplementedException());
            menu.ShowAsContext();

            e.Use();
        }

        public Rect Draw(Vector2 position, float width)
        {
            if (RuleGroup.Destination != null && !isDraggingLine)
            { 
                Vector2 destinationPoint = new Vector2(RuleGroup.Destination.Rect.x, RuleGroup.Destination.Rect.y + StateRenderer.HEADER_HEIGHT / 2);
                Color lineColor = (IsSelected) ? GUIStyles.NODE_LINE_COLOR_SELECTED : GUIStyles.NODE_LINE_COLOR;
                DrawLine(SourcePoint, destinationPoint, lineColor);
            }

            DrawRules(position, width);

            if (IsSelected)
            {
                DrawHelper.DrawBoxOutline(Rect, GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
            }

            DrawNodeKnob();

            return Rect;
        }

        private void DrawRules(Vector2 position, float width)
        {
            Rect = new Rect(position.x, position.y, width, 0);

            if (RuleGroup.Rules.Count == 0)
            {
                DrawRule(Rect, out Rect ruleRect);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }
            else
            {
                for (int i = 0; i < RuleGroup.Rules.Count; i++)
                {
                    DrawRule(Rect, out Rect ruleRect, RuleGroup.Rules[i]);
                    Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
                }
            }

            fullRect = Rect;
        }

        private void DrawNodeKnob()
        {
            Color knobColor = (RuleGroup.Destination != null) ? GUIStyles.KNOB_COLOR_OUT_LINKED : GUIStyles.KNOB_COLOR_OUT_EMPTY;

            DrawHelper.DrawRuleHandleKnob(
                new Rect(Rect.x + Rect.width + 1, Rect.y + Rect.height / 2, Rect.width, Rect.height), 
                () => {
                    if (stateRenderer.IsSelected)
                    {
                        OnDragStart(Event.current);
                    }
                }, 
                knobColor
            );
        }

        private void DrawRule(Rect groupRect, out Rect ruleRect, Rule rule = null)
        {
            ruleRect = new Rect(groupRect.x, groupRect.y + groupRect.height, groupRect.width, RULE_HEIGHT);
            string label = (rule != null) ? rule.DisplayName : EMPTY_RULE_DISPLAY_LABEL;
            GUI.Label(ruleRect, label, GUIStyles.RuleGroupStyle);
        }

        private void DrawLine(Vector2 destination, Color color)
        {
            DrawLine(SourcePoint, destination, color);
        }

        private void DrawLine(Vector2 source, Vector2 destination, Color color)
        {
            RuleGroup.line.Draw(source, destination, color, LINE_THICKNESS);
        }
        #endregion
    }
}