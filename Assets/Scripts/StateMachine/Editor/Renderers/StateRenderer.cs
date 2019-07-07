using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the <see cref="global::StateMachine.State"/> node on the <see cref="StateMachineData"/> window
    /// </summary>
    public class StateRenderer : ISelectable, IDraggable, IInspectable
    {
        public const float WIDTH = 175;
        public const float HEADER_HEIGHT = 20;
        public const float ENTRY_WIDTH = WIDTH - 20;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;
        public const float TOOLBAR_BUTTON_WIDTH = 20;
        public const float TOOLBAR_BUTTON_HEIGHT = 20;
        public const float RULE_GROUP_DIVIDER_HEIGHT = 1;

        private const string ENTRY_STRING = "ENTRY"; 

        public State State { get; private set; }
        public Rect Rect
        {
            get { return State.Rect; }
            set { State.Rect = value; }
        }

        public bool IsEntryState { get { return manager.StateMachineData.EntryState == State; } }
        public bool IsSelected { get; private set; }
        public ScriptableObject InspectableObject => State;

        private readonly RectOffset HighlightMargin = new RectOffset(3, 2, 2, 3);
        private readonly Color OutlineColor = new Color(1f, 1f, 0f, 1f);
        private readonly Color StateBackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        private readonly Color EntryPanelBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
        
        private StateMachineEditorManager manager;
        private List<RuleGroupRenderer> ruleGroupRenderers = new List<RuleGroupRenderer>();
        public Rect fullRect;
        private bool isDragged;

        public StateRenderer(State state, StateMachineEditorManager renderer)
        {
            State = state;
            manager = renderer;

            InitializeRuleRenderers();
        }

        public void InitializeRuleRenderers()
        {
            ruleGroupRenderers = new List<RuleGroupRenderer>();

            for (int i = 0; i < State.RuleGroups.Count; i++) 
            {
                ruleGroupRenderers.Add(new RuleGroupRenderer(State.RuleGroups[i], this, manager));
            }
        }

        public void ProcessEvents(Event e)
        {
            bool isInsideCanvasWindow = manager.CanvasRenderer.Contains(e.mousePosition);

            //if (IsSelected)
            //{
                ProcessRuleEvents(e);
            //}

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == (KeyCode.Delete))
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            manager.RemoveState(State);
                            e.Use();
                        }
                    }
                    break;
                case EventType.MouseDown:
                    if (isInsideCanvasWindow)
                    {
                        if (e.button == 0)
                        {
                            if (Rect.Contains(e.mousePosition))
                            {
                                if (!IsSelected)
                                {
                                    OnSelect(e);
                                    e.Use();
                                }

                                OnDragStart(e);
                            }
                            else
                            {
                                if (IsSelected)
                                {
                                    OnDeselect(e);
                                }
                            }
                        }
                        else if (e.button == 1)
                        {
                            if (IsSelected && Rect.Contains(e.mousePosition))
                            {
                                ShowContextMenu(e);
                            }
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (isDragged)
                    {
                        OnDragEnd(e);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        OnDrag(e);
                        e.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }

        public void ResetState()
        {
            RemoveAllActions();
            RemoveRuleGRoups();

            manager.Refresh();
        }

        public void RemoveAllActions()
        {
            for (int i = State.Actions.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(State.Actions[i], true);
                State.RemoveAction(State.Actions[i]);
            }
        }

        public void RemoveRuleGRoups()
        {
            ruleGroupRenderers.Clear();

            for (int i = State.RuleGroups.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(State.RuleGroups[i], true);
                State.RemoveRuleGroup(State.RuleGroups[i]);
            }
        }

        public void Draw()
        {
            if (IsEntryState)
            {
                DrawIsEntryVisual();
            }

            if (IsSelected)
            {
                Rect rect = new Rect(
                    Rect.x - HighlightMargin.right,
                    Rect.y - HighlightMargin.left,
                    Rect.width + HighlightMargin.horizontal,
                    Rect.height + HighlightMargin.vertical
                );
                DrawHelper.DrawBoxOutline(rect, OutlineColor);
                DrawAddNewRuleButton();
            }

            DrawBackground();
            DrawHeader();
            DrawRuleGroups();
        }

        private bool ProcessRuleEvents(Event e)
        {
            bool guiChanged = false;

            foreach (RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                renderer.ProcessEvents(e);
            }

            return guiChanged;
        }

        private void DrawBackground()
        {
            Color previousBackgroundColor = GUI.color;
            GUI.color = StateBackgroundColor;

            GUI.Box(fullRect, "", GUIStyles.StateHeaderStyle);
            GUI.color = previousBackgroundColor;
        }

        private void DrawHeader()
        {
            Rect = new Rect(Rect.position.x, Rect.position.y, WIDTH, HEADER_HEIGHT);

            GUI.Label(Rect, State.Title, GUIStyles.StateHeaderTitleStyle);

            DrawDividerLine(new Rect(Rect.x, Rect.y - 4, Rect.width, Rect.height), 4);

            //DrawHelper.DrawRuleHandleKnob(
            //    new Rect(Rect.x + 1, Rect.y + Rect.height / 2, Rect.width, Rect.height),
            //    null,
            //    GUIStyles.KNOB_COLOR_IN
            //);
        }

        private void DrawRuleGroups()
        {
            for (int i = 0; i < State.RuleGroups.Count; i++)
            {
                Vector2 position = new Vector2(Rect.x, Rect.y + Rect.height);
                Rect ruleRect = ruleGroupRenderers[i].Draw(position, Rect.width);

                if (i < State.RuleGroups.Count - 1)
                {
                    DrawDividerLine(ruleRect);
                }

                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }

            fullRect = Rect;
        }

        private void DrawDividerLine(Rect rect, float height = RULE_GROUP_DIVIDER_HEIGHT)
        {
            Color prevColor = GUI.color;
            GUI.color = Color.grey;
            GUI.Box(new Rect(rect.x, rect.y + rect.height, rect.width, height), "");
            GUI.color = prevColor;
        }

        private void DrawAddNewRuleButton()
        {
            Rect rect = new Rect(Rect.x + Rect.width - TOOLBAR_BUTTON_WIDTH * 2, Rect.y + Rect.height, TOOLBAR_BUTTON_WIDTH * 2, HEADER_HEIGHT);
            Rect outlineRect = new Rect(rect.x - HighlightMargin.right, rect.y, rect.width + HighlightMargin.horizontal, rect.height);
            DrawHelper.DrawBoxOutline(outlineRect, OutlineColor);

            GUI.BeginGroup(rect);
            Color prevColor = GUI.color;
            GUI.color = StateBackgroundColor;
            if (GUI.Button(new Rect(0, 0, TOOLBAR_BUTTON_WIDTH, HEADER_HEIGHT), EditorGUIUtility.IconContent("Toolbar Plus"), GUIStyles.StateToolbarButtonsStyle))
            {
                manager.Select(CreateNewRule(State));
            }
            
            //GUI.enabled = if ruleGroup is selected
            if (GUI.Button(new Rect(TOOLBAR_BUTTON_WIDTH, 0, TOOLBAR_BUTTON_WIDTH, HEADER_HEIGHT), EditorGUIUtility.IconContent("Toolbar Minus"), GUIStyles.StateToolbarButtonsStyle))
            {
                // remove selected ruleGroup
            }
            GUI.color = prevColor;
            GUI.EndGroup();
        }

        private void DrawIsEntryVisual()
        { 
            Color previousColor = GUI.color;
            Rect r = new Rect(Rect.x + Rect.width / 2 - ENTRY_WIDTH / 2, Rect.y - ENTRY_VISUAL_HEIGHT + 2, ENTRY_WIDTH, ENTRY_VISUAL_HEIGHT);

            GUI.color = EntryPanelBackgroundColor;
            GUI.Box(r, ENTRY_STRING);

            GUI.color = previousColor;
        }

        public void OnDrag(Event e)
        {
            Vector2 newPosition = Rect.position;
            newPosition += e.delta;

            // TODO: needs to take it's own size into account?
            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineCanvasRenderer.CANVAS_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineCanvasRenderer.CANVAS_HEIGHT);

            State.Rect.position = newPosition;
        }

        public void OnDragStart(Event e)
        {
            isDragged = true;
        }

        public void OnDragEnd(Event e)
        {
            isDragged = false;
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => manager.RemoveState(State));
            menu.AddItem(new GUIContent("Reset"), false, ResetState);
            menu.AddItem(new GUIContent("Add Rule"), false, () => manager.Select(CreateNewRule(State)));
            menu.ShowAsContext();

            manager.ContextMenuIsOpen = true;
            e.Use();
        }

        public void OnSelect(Event e)
        {
            GUI.changed = true;
            IsSelected = true;

            manager.Select(this);
            //style = selectedNodeStyle;
        }

        public void OnDeselect(Event e)
        {
            GUI.changed = true;
            IsSelected = false;

            foreach(RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                renderer.OnDeselect(e);
            }

            manager.Deselect(this);
            //style = defaultNodeStyle;
        }

        private RuleGroupRenderer CreateNewRule(State connectedState)
        {
            string assetFilePath = AssetDatabase.GetAssetPath(State);
            RuleGroup group = StateMachineEditorUtility.CreateObjectInstance<RuleGroup>(assetFilePath);

            //foreach(RuleGroupRenderer rend in ruleGroupRenderers)
            //{
            //    rend.OnDeselect(Event.current);
            //}

            State.AddRuleGroup(group);
            RuleGroupRenderer renderer = new RuleGroupRenderer(group, this, manager);
            ruleGroupRenderers.Add(renderer);

            renderer.OnSelect(Event.current);
            renderer.SetAsNew(Event.current);

            return renderer;
        }
    }
}