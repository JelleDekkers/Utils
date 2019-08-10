using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utils.Core.Flow.Inspector;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Renders the <see cref="Flow.State"/> node on the <see cref="StateMachineData"/> window
    /// </summary>
    public class StateRenderer : ISelectable, IDraggable
    {
        public const float WIDTH = 150;
        public const float HEADER_HEIGHT = 20;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;
        public const float TOOLBAR_BUTTON_WIDTH = 20;
        public const float TOOLBAR_BUTTON_HEIGHT = 20;
        public const float RULE_GROUP_DIVIDER_HEIGHT = 1f;

        private const float DRAG_THRESHOLD = 5f;
        private const string ENTRY_STRING = "ENTRY";
        private const float DragRounding = 20;

        public State State { get; private set; }

        public Rect Rect
        {
            get
            {
                return new Rect(State.Position, size);
            }
            private set
            {
                State.Position = value.position;
                size = value.size;
            }
        }
        private Vector2 size;

        public bool IsEntryState { get { return manager.StateMachineData.EntryState == State; } }
        public bool IsSelected { get; private set; }
        public RuleGroupRenderer SelectedRuleGroup { get; set; }

        private readonly Color HeaderBackgroundColor = new Color(0.7529413f, 0.7529413f, 0.7529413f, 0.9f);
        private readonly Color RuntimeCurrentStateHeaderBackgroundColor = new Color(0f, 1f, 0f);
        private readonly Color StateBackgroundColor = new Color(1f, 1f, 1f, 1f);
        private readonly Color EntryPanelBackgroundColor = new Color(1f, 1f, 1f, 0.7f);

        private StateMachineEditorManager manager;
        private List<RuleGroupRenderer> ruleGroupRenderers = new List<RuleGroupRenderer>();
        private Rect fullRect;
        private bool isDragging;
        private bool roundDragPosition;
        private Vector2 dragStartSelectionDif;
        private Vector2 dragStartPos;
        private bool canDrag;

        public StateRenderer(State state, StateMachineEditorManager renderer)
        {
            State = state;
            manager = renderer;

            InitializeRuleRenderers();

            StateMachineEditorUtility.RuleGroupAddedEvent += OnRuleGroupAddedEvent;
            StateMachineEditorUtility.RuleGroupRemovedEvent += OnRuleGroupRemovedEvent;

            manager.OnDisposeEvent += Dispose;
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
                    if (e.keyCode == KeyCode.Delete)
                    {
                        manager.StateMachineData.RemoveState(State);
                        e.Use();
                    }
                    else if(IsSelected && e.keyCode == KeyCode.LeftControl)
                    {
                        roundDragPosition = true;
                    }
                    break;
                case EventType.KeyUp:
                    if (IsSelected && e.keyCode == KeyCode.LeftControl)
                    {
                        roundDragPosition = false;
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
                    if (isDragging)
                    {
                        OnDragEnd(e);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragging)
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
                DrawIsEntryPanel();
            }

            DrawBackground();
            DrawHeader();
            DrawRuleGroupLinks();
            DrawRuleGroups();

            if (IsSelected)
            {
                if (SelectedRuleGroup == null)
                {
                    DrawHelper.DrawBoxOutline(Rect, GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
                }

                DrawToolbar(SelectedRuleGroup == null);
            }
        }

        private void DrawRuleGroupLinks()
        {
            for (int i = 0; i < ruleGroupRenderers.Count; i++)
            {
                ruleGroupRenderers[i].DrawLink();
            }
        }

        private void DrawHeader()
        {
            Rect = new Rect(Rect.position.x, Rect.position.y, WIDTH, HEADER_HEIGHT);

            string label = State.Title;
            float heightNeeded = Mathf.CeilToInt(GUIStyles.StateHeaderStyle.CalcHeight(new GUIContent(label), Rect.width));
            Rect = new Rect(Rect.x, Rect.y, Rect.width, (int)heightNeeded);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = (IsCurrentStateInRuntimeLogic()) ? RuntimeCurrentStateHeaderBackgroundColor : HeaderBackgroundColor;
            GUI.Box(Rect, State.Title, GUIStyles.StateHeaderStyle);
            GUI.backgroundColor = prevColor;
        }

        private void DrawBackground()
        {
            Color previousBackgroundColor = GUI.color;
            GUI.color = StateBackgroundColor;

            GUI.Box(fullRect, "", new GUIStyle("Window"));
            GUI.color = previousBackgroundColor;
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
            Color prevColor = Handles.color;
            Handles.color = Color.black;
            Vector2 p1 = new Vector3(rect.position.x + 1, rect.position.y + rect.height);
            Vector2 p2 = new Vector3(rect.position.x + rect.width, rect.position.y + rect.height);
            Handles.DrawLine(p1, p2);
            Handles.color = prevColor;
        }

        private void DrawToolbar(bool showOutline)
        {
            int buttonsAmount = 2;
            Rect rect = new Rect(Rect.x + Rect.width - TOOLBAR_BUTTON_WIDTH * buttonsAmount, Rect.y + Rect.height, TOOLBAR_BUTTON_WIDTH * buttonsAmount, HEADER_HEIGHT);

            if (showOutline)
            {
                float padding = 2;
                DrawHelper.DrawBoxOutline(new Rect(rect.x - padding, rect.y, rect.width + padding, rect.height), GUIStyles.HIGHLIGHT_OUTLINE_COLOR);
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

        private void DrawIsEntryPanel()
        { 
            Rect r = new Rect(Rect.x, Rect.y - ENTRY_VISUAL_HEIGHT + 2, Rect.width, ENTRY_VISUAL_HEIGHT);

            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = EntryPanelBackgroundColor;
            GUI.Box(r, ENTRY_STRING);
            GUI.backgroundColor = previousColor;
        }

        private void ShowContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"), false, () => manager.StateMachineData.RemoveState(State));

            if (State.TemplateActions.Count > 0)
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
            manager.Inspector.Inspect(new StateInspectorUI(manager, State));
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
            float dragDif = (dragStartPos - e.mousePosition).magnitude;
            if (dragDif > DRAG_THRESHOLD)
            {
                canDrag = true;
            }

            if(!canDrag) { return; }

            Vector2 newPosition = e.mousePosition - dragStartSelectionDif;

            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineCanvasRenderer.SCROLL_VIEW_WIDTH - fullRect.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineCanvasRenderer.SCROLL_VIEW_HEIGHT - fullRect.height);

            if (roundDragPosition)
            {
                newPosition.x = (int)Mathf.Round(newPosition.x / DragRounding) * DragRounding;
                newPosition.y = (int)Mathf.Round(newPosition.y / DragRounding) * DragRounding;
            }

            State.Position = newPosition;
        }

        public void OnDragStart(Event e)
        {
            isDragging = true;
            dragStartPos = e.mousePosition;
            dragStartSelectionDif = dragStartPos - State.Position;
            canDrag = false;
        }

        public void OnDragEnd(Event e)
        {
            isDragging = false;
            dragStartSelectionDif = Vector2.zero;
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

            manager.OnDisposeEvent -= Dispose;
        }
    }
}