using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the <see cref="global::StateMachine.State"/> node on the <see cref="StateMachine"/> window
    /// </summary>
    public class StateRenderer
    {
        public const float WIDTH = 175;
        public const float HEADER_HEIGHT = 20;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;

        private const string ENTRY_STRING = "ENTRY"; 

        public State State { get; private set; }
        public Vector2 Position => State.Position;
        public Rect Rect { get; private set; }

        public bool IsEntryState { get { return stateMachineRenderer.StateMachine.EntryState == State; } }
        public bool IsSelected { get; private set; }

        public Action<StateRenderer> SelectedEvent;
        public Action<StateRenderer> DeselectedEvent;
        public Action<StateRenderer> DeleteEvent;

        private readonly float HighlightMargin = 4;
        private readonly Color HighlightColor = Color.yellow;
        private readonly Color HeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);

        private StateMachineRenderer stateMachineRenderer;
        private GUIStyle style;
        private List<RuleRenderer> ruleRenderers = new List<RuleRenderer>();
        private bool isDragged;

        private bool drawingNewRuleLink;
        private Rule newRule;
        private RuleRenderer newRuleLinkRenderer;

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
            for (int i = 0; i < State.Rules.Count; i++) 
            {
                ruleRenderers.Add(new RuleRenderer(State.Rules[i]));
            }
        }

        public bool ProcessEvents(Event e)
        {
            bool guiChanged = false;
            bool isInsideCanvasWindow = stateMachineRenderer.CanvasWindow.Contains(e.mousePosition);


            if (IsSelected)
            {
                guiChanged = ProcessRuleEvents(e);
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (isInsideCanvasWindow)
                    {
                        if (e.button == 0)
                        {
                            if (Rect.Contains(e.mousePosition))
                            {
                                OnDragStart();
                                if (!IsSelected)
                                {
                                    OnSelect(e);
                                }
                            }
                            else
                            {
                                if (IsSelected)
                                {
                                    OnDeselect();
                                }
                            }
                        }
                    }

                    //if (e.button == 1 && rect.Contains(e.mousePosition))
                    //{
                    //    ShowDeleteStateContextMenu(e);
                    //    e.Use();
                    //}
                    break;

                case EventType.MouseUp:
                    if (isDragged)
                    {
                        OnDragEnd();
                    }
                    else if (drawingNewRuleLink)
                    {
                        StopDrawNewRuleLink(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        guiChanged = true;
                    }
                    break;
            }

            if (drawingNewRuleLink)
            {
                DrawNewRuleLink(e.mousePosition);
                guiChanged = true;
            }

            return guiChanged;
        }

        public void Reset()
        {
            ruleRenderers.Clear();
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

            if(IsSelected && !drawingNewRuleLink)
            {
                DrawAddNewRuleButton();
            }

            GUI.color = previousBackgroundColor;
        }

        private bool ProcessRuleEvents(Event e)
        {
            bool guiChanged = false;

            foreach (RuleRenderer renderer in ruleRenderers)
            {
                guiChanged = renderer.ProcessEvents(e);
            }

            return guiChanged;
        }

        private void DrawHeader()
        {
            GUI.color = HeaderBackgroundColor;
            Rect = new Rect(Position.x, Position.y, WIDTH, HEADER_HEIGHT);

            if (IsEntryState)
            {
                DrawIsEntryVisual();
            }

            // TODO: distinctive visual:
            GUI.Box(Rect, State.Title); //  GUI.Box(rect, State.Title, "window");
        }

        private void DrawRules()
        {
            for(int i = 0; i < State.Rules.Count; i++)
            {
                Vector2 position = new Vector2(Rect.x, Rect.y + Rect.height);
                Rect ruleRect = ruleRenderers[i].Draw(position, Rect.width);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRect.height);
            }
        }

        private void DrawAddNewRuleButton()
        {
            Rect r = new Rect(Rect.x, Rect.y + Rect.height, Rect.width, RuleRenderer.RULE_HEIGHT);
            if (GUI.Button(r, "Add New Rule"))
            {
                StartDrawNewRuleLink();
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

        private void Drag(Vector2 delta)
        {
            Vector2 newPosition = Position;
            newPosition += delta;

            // TODO: needs to take it's own size into account?
            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineRenderer.CANVAS_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineRenderer.CANVAS_HEIGHT);

            State.Position = newPosition;
        }

        private void OnDragStart()
        {
            isDragged = true;
        }

        private void OnDragEnd()
        {
            isDragged = false;
        }

        private void ShowDeleteStateContextMenu(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete State"), false, () => Delete());
            menu.ShowAsContext();

            e.Use();
        }

        private void OnSelect(Event e)
        {
            GUI.changed = true;
            IsSelected = true;
            SelectedEvent?.Invoke(this);
            //style = selectedNodeStyle;
        }

        private void OnDeselect()
        {
            GUI.changed = true;
            IsSelected = false;
            DeselectedEvent?.Invoke(this);
            //style = defaultNodeStyle;
        }

        private void StartDrawNewRuleLink()
        {
            drawingNewRuleLink = true;

            string assetFilePath = AssetDatabase.GetAssetPath(State);
            newRule = StateMachineEditorUtility.CreateObjectInstance<EmptyRule>(assetFilePath);
            newRuleLinkRenderer = new RuleRenderer(newRule);
        }

        private void DrawNewRuleLink(Vector2 destination)
        {
            Vector2 position = new Vector2(Rect.x, Rect.y + Rect.height);
            newRuleLinkRenderer.Draw(position, Rect.width);
            newRuleLinkRenderer.DrawLine(destination);
        }

        private void StopDrawNewRuleLink(Vector2 mousePosition)
        {
            if (stateMachineRenderer.IsStateAtPosition(mousePosition, out StateRenderer stateRenderer) && stateRenderer != null)
            {
                AddNewRule(newRule, stateRenderer.State);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(newRule, true);
            }

            drawingNewRuleLink = false;
            newRuleLinkRenderer = null;
        }

        private void AddNewRule(Rule rule, State connectedState)
        {
            State.AddRule(rule);
            rule.SetDestination(connectedState);
            ruleRenderers.Add(new RuleRenderer(rule));
        }

        private void RemoveRule(RuleRenderer ruleRenderer)
        {
            State.RemoveRule(ruleRenderer.Rule);
            ruleRenderer.OnDelete();
        }

        //private bool SelectedRule(Vector2 mousePosition, out RuleRenderer rule)
        //{
        //    for (int i = 0; i < State.Rules.Count; i++)
        //    {
        //        if (ruleRenderers[i].Rect.Contains(mousePosition))
        //        {
        //            rule = ruleRenderers[i];
        //            return true;
        //        }
        //    }

        //    rule = null;
        //    return false;
        //}

        private void Delete()
        {
            DeleteEvent?.Invoke(this);
        }
    }
}