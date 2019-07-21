using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the <see cref="StateMachine.State"/> node on the <see cref="StateMachineData"/> window
    /// </summary>
    public class StateRenderer : ISelectable, IDraggable, IDisposable
    {
        public const float WIDTH = 175;
        public const float HEADER_HEIGHT = 20;
        public const float HEADER_DIVIDER_HEIGHT = 4;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;
        public const float TOOLBAR_BUTTON_WIDTH = 20;
        public const float TOOLBAR_BUTTON_HEIGHT = 20;
        public const float RULE_GROUP_DIVIDER_HEIGHT = 1f;

        private const string ENTRY_STRING = "ENTRY"; 

        public State State { get; private set; }

        public Rect Rect
        {
            get
            {
                return new Rect(State.position, size);
            }
            private set
            {
                State.position = value.position;
                size = value.size;
            }
        }
        private Vector2 size;

        public bool IsEntryState { get { return manager.StateMachineData.EntryState == State; } }
        public bool IsSelected { get; private set; }
        public RuleGroupRenderer SelectedRuleGroup { get; set; }

        private readonly RectOffset HighlightMargin = new RectOffset(3, 2, 2, 3);
        private readonly Color StateBackgroundColor = new Color(1f, 1f, 1f, 1f);
        private readonly Color RuntimeLogicCurrentStateBackgroundColor = new Color(0.7f, 0.78f, 0.7f);
        private Rect OutlineRect
        {
            get
            {
                return new Rect(
                    Rect.x - HighlightMargin.right,
                    Rect.y - HighlightMargin.left,
                    Rect.width + HighlightMargin.horizontal,
                    Rect.height + HighlightMargin.vertical
                );
            }
        }

        private StateMachineEditorManager manager;
        private List<RuleGroupRenderer> ruleGroupRenderers = new List<RuleGroupRenderer>();
        private Rect fullRect;
        private bool isDragged;

        public StateRenderer(State state, StateMachineEditorManager renderer)
        {
            State = state;
            manager = renderer;

            InitializeRuleRenderers();

            StateMachineEditorUtility.RuleGroupAddedEvent += OnRuleGroupAddedEvent;
            StateMachineEditorUtility.RuleGroupRemovedEvent += OnRuleGroupRemovedEvent;
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

            ProcessRuleGroupEvents(e);

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == (KeyCode.Delete))
                    {
                        manager.StateMachineData.RemoveState(State);
                        e.Use();
                    }
                    break;
                case EventType.MouseDown:
                    if (isInsideCanvasWindow)
                    {
                        if (e.button == 0)
                        {
                            if (Rect.Contains(e.mousePosition))
                            {
                                if (!IsSelected || SelectedRuleGroup == null)
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
                            if (SelectedRuleGroup == null && Rect.Contains(e.mousePosition))
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

        private void ProcessRuleGroupEvents(Event e)
        {
            foreach (RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                renderer.ProcessEvents(e);
            }
        }

        public void ReorderRuleGroupRenderers(int currentIndex, int newIndex)
        {
            ruleGroupRenderers.ReorderItem(currentIndex, newIndex);
        }

        public bool IsCurrentStateInRuntimeLogic()
        {
            return Application.isPlaying && manager.Executor != null && manager.Executor.Logic != null && manager.Executor.Logic.CurrentState == State;
        }

        #region Drawing
        public void Draw()
        {
            if (IsEntryState)
            {
                DrawIsEntryVisual();
            }

            if (IsSelected)
            {
                if (SelectedRuleGroup == null)
                {
                    DrawHelper.DrawBoxOutline(OutlineRect, GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
                }

                DrawRuleToolbar(SelectedRuleGroup == null);
            }

            DrawBackground();
            DrawHeader();
            DrawRuleGroupLinks();
            DrawRuleGroups();
        }

        private void DrawRuleGroupLinks()
        {
            for (int i = 0; i < ruleGroupRenderers.Count; i++)
            {
                ruleGroupRenderers[i].DrawLink();
            }
        }

        private void DrawBackground()
        {
            Color previousBackgroundColor = GUI.color;
            GUI.color = (IsCurrentStateInRuntimeLogic()) ? RuntimeLogicCurrentStateBackgroundColor : StateBackgroundColor;

            GUI.Box(fullRect, "", GUIStyles.StateHeaderStyle);
            GUI.color = previousBackgroundColor;
        }

        private void DrawHeader()
        {
            Rect = new Rect(Rect.position.x, Rect.position.y, WIDTH, HEADER_HEIGHT);

            GUI.Label(Rect, State.Title, GUIStyles.StateHeaderTitleStyle);

            DrawDividerLine(new Rect(Rect.x, Rect.y - HEADER_DIVIDER_HEIGHT, Rect.width, Rect.height), HEADER_DIVIDER_HEIGHT);

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

        private void DrawRuleToolbar(bool showOutline)
        {
            int buttonsAmount = 2;
            Rect rect = new Rect(Rect.x + Rect.width - TOOLBAR_BUTTON_WIDTH * buttonsAmount, Rect.y + Rect.height, TOOLBAR_BUTTON_WIDTH * buttonsAmount, HEADER_HEIGHT);
            Rect outlineRect = new Rect(rect.x - HighlightMargin.right, rect.y, rect.width + HighlightMargin.horizontal, rect.height);

            if (showOutline)
            {
                DrawHelper.DrawBoxOutline(outlineRect, GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
            }

            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            Color prevColor = GUI.color;
            GUI.color = StateBackgroundColor;

            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUIStyles.StateToolbarButtonsStyle, GUILayout.MaxWidth(rect.width / buttonsAmount)))
            { 
                State.AddNewRuleGroup();
            }

            GUI.enabled = SelectedRuleGroup != null;
            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUIStyles.StateToolbarButtonsStyle, GUILayout.MaxWidth(rect.width / buttonsAmount)))
            {
                State.RemoveRuleGroup(SelectedRuleGroup.RuleGroup);
            }
            GUI.enabled = true;

            GUI.color = prevColor;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawIsEntryVisual()
        { 
            Color previousColor = GUI.backgroundColor;
            Rect r = new Rect(Rect.x, Rect.y - ENTRY_VISUAL_HEIGHT + 2, Rect.width, ENTRY_VISUAL_HEIGHT);

            GUI.backgroundColor = StateBackgroundColor;
            GUI.Box(r, ENTRY_STRING);

            GUI.backgroundColor = previousColor;
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => manager.StateMachineData.RemoveState(State));

            if (State.Actions.Count > 0)
            {
                menu.AddItem(new GUIContent("Clear Actions"), false, () => State.ClearActions());
            }

            if (State.RuleGroups.Count > 0)
            {
                menu.AddItem(new GUIContent("Clear Rules"), false, () => State.ClearRules());
            }

            menu.AddItem(new GUIContent("Add New Rulegroup"), false, () => State.AddNewRuleGroup());

            //menu.AddItem(new GUIContent("Copy State"), false, () => DataObject.CopyDataToClipboard());
            //menu.AddItem(new GUIContent("Paste Rulegroup"), false, () => throw new NotImplementedException());

            menu.ShowAsContext();

            manager.ContextMenuIsOpen = true;
            e.Use();
        }
        #endregion

        #region Events
        public void OnSelect(Event e)
        {
            GUI.changed = true;
            IsSelected = true;

            manager.Select(this);
            manager.Inspector.Inspect(State);
            manager.ReorderStateRendererToBottom(this);
        }

        public void OnDeselect(Event e)
        {
            GUI.changed = true;
            IsSelected = false;

            foreach (RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                renderer.OnDeselect(e);
            }

            manager.Deselect(this);
        }

        public void OnDrag(Event e)
        {
            Vector2 newPosition = Rect.position;
            newPosition += e.delta;

            // TODO: needs to take it's own size into account?
            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineCanvasRenderer.CANVAS_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineCanvasRenderer.CANVAS_HEIGHT);

            State.position = newPosition;
        }

        public void OnDragStart(Event e)
        {
            isDragged = true;
        }

        public void OnDragEnd(Event e)
        {
            isDragged = false;
        }

        private void OnRuleGroupAddedEvent(State state, RuleGroup group)
        {
            if(state != State) { return; }

            RuleGroupRenderer renderer = new RuleGroupRenderer(group, this, manager);
            ruleGroupRenderers.Add(renderer);

            if (SelectedRuleGroup != null)
            {
                SelectedRuleGroup.OnDeselect(Event.current);
            }

            SelectedRuleGroup = renderer;
            renderer.OnSelect(Event.current);
        }

        private void OnRuleGroupRemovedEvent(State state, RuleGroup ruleGroup)
        {
            if(state != State) { return; }

            SelectedRuleGroup = null;
            InitializeRuleRenderers();
            OnSelect(Event.current);

            return;
        }

        public void OnStateResetEvent(State state)
        {
            if (manager.Selection == state as ISelectable)
            {
                manager.Inspector.Refresh();
            }
        }
        #endregion

        public void Dispose()
        {
            StateMachineEditorUtility.RuleGroupAddedEvent -= OnRuleGroupAddedEvent;
            StateMachineEditorUtility.RuleGroupRemovedEvent -= OnRuleGroupRemovedEvent;
        }
    }
}