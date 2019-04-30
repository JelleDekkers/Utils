﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the <see cref="global::StateMachine.State"/> node on the <see cref="StateMachineData"/> window
    /// </summary>
    public class StateRenderer : ISelectable, IDraggable
    {
        public const float WIDTH = 175;
        public const float HEADER_HEIGHT = 20;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;

        private const string ENTRY_STRING = "ENTRY"; 

        public State State { get; private set; }
        public Rect Rect
        {
            get { return State.Rect; }
            set { State.Rect = value; }
        }

        public bool IsEntryState { get { return stateMachineRenderer.StateMachine.EntryState == State; } }
        public bool IsSelected { get; private set; }

        //public Action<ISelectable> SelectedEvent;
        //public Action<ISelectable> DeselectedEvent;
        //public Action<StateRenderer> DeleteEvent;

        private readonly float HighlightMargin = 4;
        private readonly Color HighlightColor = Color.yellow;
        private readonly Color HeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);

        private StateMachineRenderer stateMachineRenderer;
        private List<RuleGroupRenderer> ruleGroupRenderers = new List<RuleGroupRenderer>();
        private GUIStyle style;
        private bool isDragged;

        public StateRenderer(State state, StateMachineRenderer renderer)
        {
            State = state;
            stateMachineRenderer = renderer;

            InitializeRuleRenderers();

            // TODO: tidy up in its own function? and vars
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void InitializeRuleRenderers()
        {
            for (int i = 0; i < State.RuleGroups.Count; i++) 
            {
                ruleGroupRenderers.Add(new RuleGroupRenderer(State.RuleGroups[i], this, stateMachineRenderer));
            }
        }

        public bool ProcessEvents(Event e)
        {
            bool guiChanged = false;
            bool isInsideCanvasWindow = stateMachineRenderer.CanvasWindow.Contains(e.mousePosition);

            switch (e.type)
            {
                case EventType.KeyDown:
                    if (e.keyCode == (KeyCode.Delete))
                    {
                        if (Rect.Contains(e.mousePosition))
                        {
                            stateMachineRenderer.RemoveState(State);
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
                                OnDragStart(e);
                                if (!IsSelected)
                                {
                                    OnSelect(e);
                                    e.Use();
                                }
                            }
                            else
                            {
                                if (IsSelected)
                                {
                                    OnDeselect(e);
                                }
                            }
                        }
                        //else if (e.button == 1)
                        //{
                        //    if (Rect.Contains(e.mousePosition))
                        //    {
                        //        ShowContextMenu(e);
                        //    }
                        //}
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
                        guiChanged = true;
                    }
                    break;
            }

            if(IsSelected && ProcessRuleEvents(e))
            {
                guiChanged = true;
            }

            return guiChanged;
        }

        public void ResetState()
        {
            ResetActions();
            ResetRuleGRoups();

            stateMachineRenderer.Refresh();
            stateMachineRenderer.Inspector.Refresh();
        }

        public void ResetActions()
        {
            for (int i = State.Actions.Count - 1; i >= 0; i--)
            {
                State.RemoveAction(State.Actions[i]);
                UnityEngine.Object.DestroyImmediate(State.Actions[i], true);
                UnityEngine.Object.DestroyImmediate(State.RuleGroups[i], true);
            }
        }

        public void ResetRuleGRoups()
        {
            ruleGroupRenderers.Clear();

            for (int i = State.RuleGroups.Count - 1; i >= 0; i--)
            {
                State.RemoveRuleGroup(State.RuleGroups[i]);
            }
        }

        public void Draw()
        {
            if (IsSelected)
            {
                DrawHighlight(HighlightColor);
            }

            Color previousBackgroundColor = GUI.color;

            DrawHeader();
            DrawRules();

            if(IsSelected)// && !drawingNewRuleLink)
            {
                DrawAddNewRuleButton();
            }

            DrawHelper.DrawLinkNode(new Vector2(Rect.x, Rect.y + Rect.height / 2));

            GUI.color = previousBackgroundColor;
        }

        private bool ProcessRuleEvents(Event e)
        {
            bool guiChanged = false;

            foreach (RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                if(renderer.ProcessEvents(e))
                {
                    guiChanged = true;
                }
            }

            return guiChanged;
        }

        private void DrawHeader()
        {
            GUI.color = HeaderBackgroundColor;
            Rect = new Rect(Rect.position.x, Rect.position.y, WIDTH, HEADER_HEIGHT);

            if (IsEntryState)
            {
                DrawIsEntryVisual();
            }

            // TODO: distinctive visual:
            GUI.Box(Rect, State.Title); //  GUI.Box(rect, State.Title, "window");
        }

        private void DrawRules()
        {
            for(int i = 0; i < State.RuleGroups.Count; i++)
            {
                Vector2 position = new Vector2(Rect.x, Rect.y + Rect.height);
                Rect ruleRect = ruleGroupRenderers[i].Draw(position, Rect.width);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }
        }

        private void DrawAddNewRuleButton()
        {
            Rect r = new Rect(Rect.x, Rect.y + Rect.height, Rect.width, HEADER_HEIGHT);
            if (GUI.Button(r, "Add New Rule"))
            {
                stateMachineRenderer.Inspector.Inspect(CreateNewRule(State));
            }
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

        private void DrawIsEntryVisual()
        { 
            Rect r = new Rect(Rect.x, Rect.y - ENTRY_VISUAL_HEIGHT, Rect.width, ENTRY_VISUAL_HEIGHT);
            GUI.Box(r, ENTRY_STRING);
        }

        public void OnDrag(Event e)
        {
            Vector2 newPosition = Rect.position;
            newPosition += e.delta;

            // TODO: needs to take it's own size into account?
            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineRenderer.CANVAS_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineRenderer.CANVAS_HEIGHT);

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
            menu.AddItem(new GUIContent("Delete"), false, () => stateMachineRenderer.RemoveState(State));
            menu.AddItem(new GUIContent("Reset"), false, ResetState);
            menu.AddItem(new GUIContent("Add Rule"), false, () => stateMachineRenderer.Inspector.Inspect(CreateNewRule(State)));
            menu.ShowAsContext();

            e.Use();
        }

        public void OnSelect(Event e)
        {
            GUI.changed = true;
            IsSelected = true;
            //SelectedEvent?.Invoke(this);
            //style = selectedNodeStyle;

            stateMachineRenderer.Inspector.Inspect(State);
        }

        public void OnDeselect(Event e)
        {
            GUI.changed = true;
            IsSelected = false;

            foreach(RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                renderer.OnDeselect(e);
            }

            //DeselectedEvent?.Invoke(this);

            //style = defaultNodeStyle;
        }

        private RuleGroup CreateNewRule(State connectedState)
        {
            string assetFilePath = AssetDatabase.GetAssetPath(State);
            RuleGroup group = StateMachineEditorUtility.CreateObjectInstance<RuleGroup>(assetFilePath);

            State.AddRuleGroup(group);
            RuleGroupRenderer renderer = new RuleGroupRenderer(group, this, stateMachineRenderer);
            ruleGroupRenderers.Add(renderer);

            renderer.OnSelect(Event.current);
            renderer.SetAsNew();

            return group;
        }

        public void RemoveRuleGroup(RuleGroup group)
        {
            foreach(RuleGroupRenderer renderer in ruleGroupRenderers)
            {
                if(renderer.RuleGroup == group)
                {
                    RemoveRuleGroup(renderer);
                    return;
                }
            }
        }

        public void RemoveRuleGroup(RuleGroupRenderer ruleRenderer)
        {
            State.RemoveRuleGroup(ruleRenderer.RuleGroup);
            ruleGroupRenderers.Remove(ruleRenderer);

            if(stateMachineRenderer.Inspector.InspectedObject == ruleRenderer.RuleGroup)
            {
                stateMachineRenderer.Inspector.Clear();
            }
        }
    }
}