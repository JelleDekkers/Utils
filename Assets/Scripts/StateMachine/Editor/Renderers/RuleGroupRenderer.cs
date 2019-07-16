using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Class for rendering <see cref="RuleGroup"/>s on <see cref="StateMachineEditorManager"/>
    /// </summary>
    public class RuleGroupRenderer : NodeRenderer<RuleGroup>, ISelectable, IDraggable
    {
        private const float RULE_HEIGHT = StateRenderer.HEADER_HEIGHT;
        private const float LINE_THICKNESS = 3f;
        private const string EMPTY_RULE_DISPLAY_LABEL = "TRUE";

        public RuleGroup DataObject { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }
        
        private Vector2 SourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }
        
        private StateRenderer stateRenderer;
        private StateMachineEditorManager manager;
        private Rect fullRect;
        private bool isDraggingLine;
       
        public RuleGroupRenderer(RuleGroup ruleGroup, StateRenderer state, StateMachineEditorManager stateMachine)
        {
            DataObject = ruleGroup;
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
                        stateRenderer.DataObject.RemoveRuleGroup(DataObject);
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

        private void ReorderRuleGroup(ContextMenu.ReorderDirection direction)
        {
            int currentIndex = stateRenderer.DataObject.RuleGroups.IndexOf(DataObject);
            int newIndex = currentIndex + (int)direction;
            if (newIndex >= 0 && newIndex < stateRenderer.DataObject.RuleGroups.Count)
            {
                stateRenderer.DataObject.RuleGroups.ReorderItem(currentIndex, newIndex);
                stateRenderer.ReorderRuleGroupRenderers(currentIndex, newIndex);
            }
        }

        #region Events
        public void OnSelect(Event e)
        {
            IsSelected = true;
            manager.Select(this);
            stateRenderer.SelectedRuleGroup = this;
            manager.Inspector.Inspect(DataObject);
            GUI.changed = true;
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
            manager.Deselect(this);
            isDraggingLine = false;

            if(stateRenderer.SelectedRuleGroup == this)
            {
                stateRenderer.SelectedRuleGroup = null;
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
                if (stateRenderer.DataObject == this.stateRenderer.DataObject)
                {
                    DataObject.SetDestination(null);
                }
                else
                {
                    DataObject.SetDestination(stateRenderer.DataObject);
                }
            }
            else
            {
                DataObject.SetDestination(null);
            }

            isDraggingLine = false;
            GUI.changed = true;
        }
        #endregion

        #region Drawing
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => stateRenderer.DataObject.RemoveRuleGroup(DataObject));

            if (DataObject.Rules.Count > 0)
            {
                menu.AddItem(new GUIContent("Clear"), false, () => DataObject.Clear());
            }

            //menu.AddItem(new GUIContent("Copy"), false, () => DataObject.CopyDataToClipboard());
            // if copy/paste buffer contains of type RuleGroup
            //menu.AddItem(new GUIContent("Paste"), false, () => Debug.Log(DataObject.PasteFromClipboard()));

            menu.AddItem(new GUIContent("Move up"), false, () => ReorderRuleGroup(ContextMenu.ReorderDirection.Up));
            menu.AddItem(new GUIContent("Move down"), false, () => ReorderRuleGroup(ContextMenu.ReorderDirection.Down));
            menu.ShowAsContext();

            e.Use();
        }

        public Rect Draw(Vector2 position, float width)
        {
            if (DataObject.Destination != null && !isDraggingLine)
            { 
                Vector2 destinationPoint = new Vector2(DataObject.Destination.position.x, DataObject.Destination.position.y + StateRenderer.HEADER_HEIGHT / 2);
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

            if (DataObject.Rules.Count == 0)
            {
                DrawLink(Rect, out Rect ruleRect);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }
            else
            {
                for (int i = 0; i < DataObject.Rules.Count; i++)
                {
                    DrawLink(Rect, out Rect ruleRect, DataObject.Rules[i]);
                    Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
                }
            }

            fullRect = Rect;
        }

        private void DrawNodeKnob()
        {
            Color knobColor = (DataObject.Destination != null) ? GUIStyles.KNOB_COLOR_OUT_LINKED : GUIStyles.KNOB_COLOR_OUT_EMPTY;

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

        private void DrawLink(Rect groupRect, out Rect ruleRect, Rule rule = null)
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
            DataObject.line.Draw(source, destination, color, LINE_THICKNESS);
        }
        #endregion
    }
}