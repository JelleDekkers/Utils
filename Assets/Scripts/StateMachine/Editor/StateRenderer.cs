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
        public const float RULE_HEIGHT = HEADER_HEIGHT;
        public const float ENTRY_VISUAL_HEIGHT = HEADER_HEIGHT;

        private const string ENTRY_STRING = "ENTRY";

        public State State { get; private set; }
        public Vector2 Position
        {
            get { return State.Position; }
            set { State.Position = value; }
        }
        public Rect Rect { get; private set; }

        public bool IsEntryState { get { return stateMachineRenderer.StateMachine.EntryState == State; } }
        public bool IsSelected { get; private set; }

        public Action<StateRenderer> SelectedEvent;
        public Action<StateRenderer> DeselectedEvent;
        public Action<StateRenderer> DeleteEvent;

        private readonly float HighlightMargin = 3;
        private readonly Color HighlightColor = Color.yellow;
        private readonly Color HeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);

        private StateMachineRenderer stateMachineRenderer;
        private GUIStyle style;
        private List<Rect> ruleRects = new List<Rect>();
        private List<LinkRenderer> linkRenderers = new List<LinkRenderer>();
        private bool isDragged;

        private bool drawingNewRuleLink;
        private LinkRenderer newRuleLinkRenderer;

        public StateRenderer(State state, StateMachineRenderer renderer)
        {
            State = state;
            stateMachineRenderer = renderer;

            InitializeRuleDrawers();

            // TODO: tidy up in its own function?
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void InitializeRuleDrawers()
        {
            // foreach state, check rules en draw them
            for (int i = 0; i < State.Rules.Count; i++)
            {
                ruleRects.Add(new Rect());
                linkRenderers.Add(new LinkRenderer(State.Rules[i].Link));
            }
        }

        public bool ProcessEvents(Event e)
        {
            bool guiChanged = false;
            bool isInsideCanvasWindow = stateMachineRenderer.CanvasWindow.Contains(e.mousePosition);

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
                                else
                                {
                                    if (SelectedRule(e.mousePosition, out Rule rule))
                                    {
                                        ShowRuleInInspector(rule);
                                    }
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
            ruleRects = new List<Rect>();
            linkRenderers = new List<LinkRenderer>();
        }

        public void Draw()
        {
            if (IsSelected)
            {
                DrawHighlight();
            }

            Color previousBackgroundColor = GUI.color;

            DrawHeader();
            DrawRules();
            State.ConnectionPointPosition = new Vector2(Rect.x, Rect.y + HEADER_HEIGHT / 2);
            DrawLinks();

            if(IsSelected && !drawingNewRuleLink)
            {
                DrawAddNewRuleButton();
            }

            GUI.color = previousBackgroundColor;
            GUI.color = previousBackgroundColor;
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
                ruleRects[i] = new Rect(Rect.x, Rect.y + Rect.height, Rect.width, RULE_HEIGHT);
                GUI.Box(ruleRects[i], State.Rules[i].DisplayName);
                Rect = new Rect(Rect.x, Rect.y, Rect.width, Rect.height + ruleRects[i].height);

                State.Rules[i].ConnectionPointPosition = new Vector2(ruleRects[i].position.x + ruleRects[i].width, ruleRects[i].position.y + ruleRects[i].height / 2);
            }
        }

        private void DrawLinks()
        {
            for (int i = 0; i < linkRenderers.Count; i++)
            {
                linkRenderers[i].Draw();
            }
        }

        private void DrawAddNewRuleButton()
        {
            Rect r = new Rect(Rect.x, Rect.y + Rect.height, Rect.width, RULE_HEIGHT);
            if (GUI.Button(r, "Add New Rule"))
            {
                StartDrawNewRuleLink();
            }
        }

        private void DrawHighlight()
        {
            Color previousColor = GUI.color;

            Rect r = new Rect(
                Rect.x - HighlightMargin / 2,
                Rect.y - HighlightMargin / 2,
                Rect.width + HighlightMargin,
                Rect.height + HighlightMargin);

            GUI.color = HighlightColor;
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

            // TODO: cleanup, zijn niet beide nodig?:
            Position = newPosition;
            State.Position = Position;
            State.ConnectionPointPosition = new Vector2(Rect.x, Rect.y + Rect.height / 2);
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
            Rule newRule = StateMachineEditorUtility.CreateObjectInstance<EmptyRule>(assetFilePath);
            Link link = new Link(State, null);
            newRuleLinkRenderer = new LinkRenderer(link);
        }

        private void StopDrawNewRuleLink(Vector2 mousePosition)
        {
            if (stateMachineRenderer.IsStateAtPosition(mousePosition, out StateRenderer stateRenderer))
            {
                if(stateRenderer != this)
                {
                    CreateNewRule(new EmptyRule(), stateRenderer.State);
                }
            }

            drawingNewRuleLink = false;
            newRuleLinkRenderer = null;
        }

        private void DrawNewRuleLink(Vector2 destination)
        {
            newRuleLinkRenderer.Draw(State.Position, destination);
        }

        private void CreateNewRule(Rule rule, State connectedState)
        {
            State.AddRule(rule);
            Link link = new Link(rule, connectedState);
            rule.SetLink(link);

            linkRenderers.Add(new LinkRenderer(link));
            ruleRects.Add(new Rect(Rect.x, Rect.y, Rect.width, RULE_HEIGHT));
        }

        private void RemoveRule()
        {
            throw new NotImplementedException();
        }

        private bool SelectedRule(Vector2 mousePosition, out Rule rule)
        {
            for (int i = 0; i < State.Rules.Count; i++)
            {
                if(ruleRects[i].Contains(mousePosition))
                {
                    rule = State.Rules[i];
                    return true;
                }
            }

            rule = null;
            return false;
        }

        private void ShowRuleInInspector(Rule rule)
        {
            // TODO: inspector show rule
        }

        private void Delete()
        {
            DeleteEvent?.Invoke(this);
        }
    }
}