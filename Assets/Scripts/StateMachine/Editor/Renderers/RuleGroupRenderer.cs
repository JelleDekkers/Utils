﻿using UnityEditor;
using UnityEngine;
using Utils.Core.Flow.Inspector;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for rendering <see cref="Flow.RuleGroup"/>s on <see cref="StateMachineUIImplementation"/>
    /// </summary>
    public class RuleGroupRenderer : ISelectable, IDraggable
    {
        private const float RULE_HEIGHT_SINGLE_LINE = 20f;
        private const float LINE_THICKNESS = 3f;
        private const string EMPTY_RULE_DISPLAY_LABEL = "TRUE";
        private readonly Color ValidRuleTextColor = new Color(0, 0.7f, 0);

        public RuleGroup RuleGroup { get; private set; }
        public Rect Rect { get; private set; }
        public bool IsSelected { get; private set; }
        
        private Vector2 LinkSourcePoint { get { return new Vector2(Rect.position.x + Rect.width, Rect.position.y + Rect.height / 2); } }
        
        public StateRenderer stateRenderer;
        private StateMachineUIImplementation editorUI;
        private Rect fullRect;
        private bool isDraggingLink;
        private LinkRenderer linkRenderer;

        public RuleGroupRenderer(RuleGroup ruleGroup, StateRenderer stateRenderer, StateMachineUIImplementation editorUI)
        {
            RuleGroup = ruleGroup;
            this.stateRenderer = stateRenderer;
            this.editorUI = editorUI;

            Rect = new Rect();
            linkRenderer = new LinkRenderer(RuleGroup.linkData);
        }

        public void ProcessEvents(Event e)
        {
            if (isDraggingLink)
            {
                ProcessEventsOnDraggingLink(e);
            }

            if (IsSelected)
            {
                linkRenderer.ProcessEvents(e);
            }

            if(RuleGroup.Destination != null)
            {
                ProcessEventsOnConnectedLink(e);
            }

            if(stateRenderer.IsSelected)
            {
                ProcessEventOnSelected(e);
            }
        }

        private void ProcessEventsOnConnectedLink(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (linkRenderer.IsHovering(e.mousePosition))
                {
                    OnSelect(e);
                    e.Use();
                }
            }
        }

        private void ProcessEventsOnDraggingLink(Event e)
        {
            OnDrag(e);

            if ((e.type == EventType.MouseDown && e.button == 0) || (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape))
            {
                if (isDraggingLink)
                {
                    OnDragEnd(e);
                    e.Use();
                }
            }
        }

        private void ProcessEventOnSelected(Event e)
        {
            switch (e.type)
            {
                case EventType.KeyDown:
                    if (IsSelected && e.keyCode == (KeyCode.Delete))
                    {
                        stateRenderer.Node.RemoveRuleGroup(RuleGroup);
                        e.Use();
                        return;
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
            int currentIndex = stateRenderer.Node.RuleGroups.IndexOf(RuleGroup);
            int newIndex = currentIndex + (int)direction;
            if (newIndex >= 0 && newIndex < stateRenderer.Node.RuleGroups.Count)
            {
                stateRenderer.Node.RuleGroups.ReorderItem(currentIndex, newIndex);
                stateRenderer.ReorderRuleGroupRenderers(currentIndex, newIndex);
            }
        }

        #region Events
        public void OnSelect(Event e)
        {
            stateRenderer.OnSelect(e);

            IsSelected = true;
            editorUI.Select(this);
            stateRenderer.SelectedRuleGroup = this;
            editorUI.Inspector.Inspect(new RuleGroupInspector(editorUI, stateRenderer.Node, RuleGroup));
            GUI.changed = true;
        }

        public void OnDeselect(Event e)
        {
            IsSelected = false;
            editorUI.Deselect(this);
            isDraggingLink = false;

            if(stateRenderer.SelectedRuleGroup == this)
            {
                stateRenderer.SelectedRuleGroup = null;
            }

            GUI.changed = true;
        }

        public void OnDragStart(Event e)
        {
            isDraggingLink = true;
        }

        public void OnDrag(Event e)
        {
            DrawLink(e.mousePosition, GUIStyles.LINK_COLOR_SELECTED);
            GUI.changed = true;
        }

        public void OnDragEnd(Event e)
        {
            if (editorUI.IsStateAtPosition(e.mousePosition, out StateRenderer stateRenderer))
            {
                RuleGroup.SetDestination(stateRenderer.Node as State);   
            }
            else
            {
                RuleGroup.SetDestination(null);
            }

            linkRenderer.Reset();
            isDraggingLink = false;
            GUI.changed = true;
        }

        private void DeleteRule()
        {
            RuleGroup.linkData = new LinkData();
            stateRenderer.Node.RemoveRuleGroup(RuleGroup);
        }
        #endregion

        #region Drawing
        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, DeleteRule);

            menu.AddSeparator("");

            if (RuleGroup.TemplateRules.Count > 0)
            {
                menu.AddItem(new GUIContent("Clear"), false, () => RuleGroup.Clear());
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Clear"));
            }

            if (RuleGroup.Destination != null)
            {
                menu.AddItem(new GUIContent("Reset link curve"), false, () => linkRenderer.Reset());
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Reset link curve"));
            }

            //menu.AddItem(new GUIContent("Copy"), false, () => DataObject.CopyDataToClipboard());
            // if copy/paste buffer contains of type RuleGroup
            //menu.AddItem(new GUIContent("Paste"), false, () => Debug.Log(DataObject.PasteFromClipboard()));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Move up"), false, () => ReorderRuleGroup(ContextMenu.ReorderDirection.Up));
            menu.AddItem(new GUIContent("Move down"), false, () => ReorderRuleGroup(ContextMenu.ReorderDirection.Down));
            menu.ShowAsContext();

            e.Use();
        }

        public Rect Draw(Vector2 position, float width)
        {
            DrawRules(position, width);

            if (IsSelected)
            {
                DrawHelper.DrawBoxOutline(Rect, GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
            }

            DrawNodeKnob();

            return Rect;
        }

        public void DrawLink()
        {
            if (RuleGroup.Destination != null && !isDraggingLink)
            {
                Vector2 destinationPoint = new Vector2(RuleGroup.Destination.Position.x, RuleGroup.Destination.Position.y + StateRenderer.HEADER_HEIGHT / 2);
                Color color = (IsSelected) ? GUIStyles.LINK_COLOR_SELECTED : GUIStyles.LINK_COLOR;
                DrawLink(LinkSourcePoint, destinationPoint, color);
            }
        }
        private void DrawRules(Vector2 position, float width)
        {
            Rect = new Rect(position.x, position.y, width, 0);

            if (RuleGroup.TemplateRules.Count == 0)
            {
                DrawRule(Rect, out Rect ruleRect);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }
            else
            {
                if (Application.isPlaying && stateRenderer.IsCurrentlyRunning())
                {
                    for (int i = 0; i < RuleGroup.RuntimeRules.Count; i++)
                    {
                        DrawRule(Rect, out Rect ruleRect, RuleGroup.RuntimeRules[i]);
                        Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
                    }
                }
                else
                {
                    for (int i = 0; i < RuleGroup.TemplateRules.Count; i++)
                    {
                        DrawRule(Rect, out Rect ruleRect, RuleGroup.TemplateRules[i]);
                        Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
                    }
                }
            }

            fullRect = Rect;
        }

        private void DrawRule(Rect groupRect, out Rect ruleRect, Rule rule = null)
        {
            GUIStyle style = GUIStyles.RuleGroupStyle;
            string label = (rule != null) ? rule.DisplayName : EMPTY_RULE_DISPLAY_LABEL;

            float heightNeeded = Mathf.CeilToInt(style.CalcHeight(new GUIContent(label), groupRect.width));
            ruleRect = new Rect(groupRect.x, groupRect.y + groupRect.height, groupRect.width, (int)heightNeeded);

            style.normal.textColor = Color.black;
            if (Application.isPlaying && stateRenderer.IsCurrentlyRunning()) 
            {
                if (rule == null || rule.IsValid)
                {
                    style.normal.textColor = ValidRuleTextColor;
                }
            }

            GUI.Box(ruleRect, label, style);
        }

        private void DrawNodeKnob()
        {
            Color knobColor = (RuleGroup.Destination != null) ? GUIStyles.KNOB_COLOR_LINKED : GUIStyles.KNOB_COLOR_EMPTY;

            DrawHelper.DrawRuleHandleKnob(
                new Rect(Rect.x + Rect.width - 0.5f, Rect.y + Rect.height / 2, Rect.width, Rect.height),
                () => OnDragStart(Event.current),
                knobColor
            );
        }

        private void DrawLink(Vector2 destination, Color color)
        {
            DrawLink(LinkSourcePoint, destination, color);
        }

        private void DrawLink(Vector2 source, Vector2 destination, Color color)
        {
            linkRenderer.Draw(source, destination, color, LINE_THICKNESS, IsSelected || isDraggingLink);
        }
        #endregion
    }
}