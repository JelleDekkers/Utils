using System;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Renders the <see cref="State"/> node on the <see cref="StateMachine"/> window
    /// </summary>
    public class StateRenderer
    {
        public const float WIDTH = 175;
        public const float HEADER_HEIGHT = 20;

        private const string START_STRING = "START";

        public State State { get; private set; }
        public Vector2 Position
        {
            get { return State.Position; }
            set { State.Position = value; }
        }
        public bool IsStartState { get { return stateMachineRenderer.StateMachine.StartState == State; } }
        public bool IsSelected { get; private set; }

        public Action<StateRenderer> SelectedEvent;
        public Action<StateRenderer> DeselectedEvent;
        public Action<StateRenderer> DeleteEvent;

        private readonly float HighlightMargin = 3;
        private readonly Color HighlightColor = Color.yellow;
        private readonly Color HeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);

        private StateMachineRenderer stateMachineRenderer;
        private GUIStyle style;
        private bool isDragged;
        private Rect rect;

        public StateRenderer(State state, StateMachineRenderer renderer)
        {
            State = state;
            stateMachineRenderer = renderer;

            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        public void Draw()
        {
            if (IsSelected)
            {
                DrawHighlight();
            }

            Color previousBackgroundColor = GUI.color;

            DrawHeader();

            GUI.color = previousBackgroundColor;
        }

        private void DrawHeader()
        {
            GUI.color = HeaderBackgroundColor;
            rect = new Rect(Position.x, Position.y, WIDTH, HEADER_HEIGHT);

            if (IsStartState)
            {
                DrawStartVisual();
            }

            GUI.Box(rect, State.Title); //  GUI.Box(rect, State.Title, "window");
        }

        private void DrawStartVisual()
        {
            Rect r = rect;
            r.y -= HEADER_HEIGHT - 2;
            GUI.Box(r, "START");
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (stateMachineRenderer.CanvasWindow.Contains(e.mousePosition))
                        {
                            if (rect.Contains(e.mousePosition))
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
                    OnDragEnd();
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void Drag(Vector2 delta)
        {
            Vector2 newPosition = Position;
            newPosition += delta;

            // TODO: needs to take it's own size into account?
            newPosition.x = Mathf.Clamp(newPosition.x, 0, StateMachineRenderer.CANVAS_WIDTH);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, StateMachineRenderer.CANVAS_HEIGHT);

            Position = newPosition;
        }

        private void OnDragStart()
        {
            isDragged = true;
        }

        private void OnDragEnd()
        {
            isDragged = false;
        }

        private void DrawHighlight()
        {
            Color previousColor = GUI.color;

            Rect r = new Rect(
                rect.x - HighlightMargin / 2,
                rect.y - HighlightMargin / 2,
                rect.width + HighlightMargin,
                rect.height + HighlightMargin);

            GUI.color = HighlightColor;
            GUI.Box(r, "");

            GUI.color = previousColor;
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

        private void Delete()
        {
            DeleteEvent?.Invoke(this);
        }
    }
}