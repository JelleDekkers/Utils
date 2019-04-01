using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class BehaviourEditor : EditorWindow
    {
        public enum UserActions
        {
            addState,
            addTransitionNode,
            deleteNode,
            commentNode
        }

        public static BehaviourGraph currentGraph;

        private static GraphNode graphNode;
        private static List<BaseNode> windows = new List<BaseNode>();

        private Vector3 mousePosition;
        private bool makeTransition = false;
        private bool clickedOnWindow;
        private int selectedNodeIndex = -1;
        private BaseNode selectedNode;

        [MenuItem("Tools/Behaviour Editor")]
        private static void ShowWindow()
        {
            BehaviourEditor editor = EditorWindow.GetWindow<BehaviourEditor>();
            editor.minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            if(graphNode == null)
            {
                graphNode = CreateInstance<GraphNode>();
                graphNode.windowRect = new Rect(10, position.height * 0.7f, 200, 100);
                graphNode.windowTitle = "Graph";
            }

            windows.Clear();
            windows.Add(graphNode);
            LoadGraph();
        }

        private void OnGUI()
        {
            mousePosition = Event.current.mousePosition;
            UserInput(Event.current);
            DrawWindows();
        }

        void DrawWindows()
        {
            BeginWindows();
            foreach (BaseNode node in windows)
            {
                node.DrawCurve();
            }

            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
            }
            EndWindows();
        }

        private void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();
        }

        private void UserInput(Event e)
        {
            if (e.button == 0 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    LeftClick(e);
                }

                if(e.type == EventType.MouseDrag)
                {
                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i].windowRect.Contains(e.mousePosition))
                        {
                            if (currentGraph != null)
                            {
                                currentGraph.SetNode(windows[i]);
                            }

                            break;
                        }
                    }
                }
            }

            if (e.button == 1 && !makeTransition)
            {
                if (e.type == EventType.MouseDown)
                {
                    RightClick(e);
                }
            }
        }

        private void LeftClick(Event e)
        {

        }

        private void RightClick(Event e)
        {
            selectedNodeIndex = -1;
            clickedOnWindow = false;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];
                    selectedNodeIndex = i;
                    break;
                }
            }

            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        private void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");

            if(currentGraph != null)
            {
                menu.AddItem(new GUIContent("Add New State"), false, OnNewItemAdded, UserActions.addState);
                menu.AddItem(new GUIContent("Add Comment"), false, OnNewItemAdded, UserActions.commentNode);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();

            if (selectedNode is StateNode)
            {
                StateNode stateNode = (StateNode)selectedNode;

                if (stateNode.currentState != null)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Add Transition"), false, OnNewItemAdded, UserActions.addTransitionNode);
                }
                else
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("Add Transition"));
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, OnNewItemAdded, UserActions.deleteNode);
            }

            if (selectedNode is TransitionNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, OnNewItemAdded, UserActions.deleteNode);
            }

            if (selectedNode is CommentNode)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, OnNewItemAdded, UserActions.deleteNode);
            }

            menu.ShowAsContext();
            e.Use();
        }

        private void OnNewItemAdded(object o)
        {
            UserActions a = (UserActions)o;

            switch (a)
            {
                case UserActions.addState:
                    AddStateNode(mousePosition);
                    break;

                case UserActions.addTransitionNode:
                    if(selectedNode is StateNode)
                    {
                        StateNode from = (StateNode)selectedNode;
                        Transition transition = from.AddTransition();
                        AddTransitionNode(from.currentState.transitions.Count, transition, from);
                    }
                    break;

                case UserActions.commentNode:
                    AddCommentNode(mousePosition);
                    break;

                case UserActions.deleteNode:
                    if (selectedNode is StateNode)
                    {
                        StateNode target = (StateNode)selectedNode;
                        target.ClearReferences();
                        windows.Remove(target);
                    }

                    if(selectedNode is TransitionNode)
                    {
                        TransitionNode target = (TransitionNode)selectedNode;
                        windows.Remove(target);

                        if(target.enterState.currentState.transitions.Contains(target.targetTransition))
                        {
                            target.enterState.currentState.transitions.Remove(target.targetTransition);
                        }
                    }

                    if(selectedNode is CommentNode)
                    {
                        windows.Remove(selectedNode);
                    }

                    break;
                default:
                    break;
            }
        }

        public static StateNode AddStateNode(Vector2 position)
        {
            StateNode stateNode = CreateInstance<StateNode>();
            stateNode.windowRect = new Rect(position.x, position.y, 200, 300);
            stateNode.windowTitle = "State";
            windows.Add(stateNode);
            //currentGraph.SetStateNode(stateNode);

            return stateNode;
        }

        public static CommentNode AddCommentNode(Vector2 position)
        {
            CommentNode commentNode = CreateInstance<CommentNode>();
            commentNode.windowRect = new Rect(position.x, position.y, 200, 100);
            commentNode.windowTitle = "Comment";
            windows.Add(commentNode);

            return commentNode;
        }

        public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from)
        {
            Rect fromRect = from.windowRect;
            fromRect.x += 50;
            float targetY = fromRect.y - fromRect.height;

            if(from.currentState != null)
            {
                targetY += index * 100;
            }

            fromRect.y = targetY;
            fromRect.x = 200 + 100;
            fromRect.y += (fromRect.height * 0.7f);

            Vector2 position = new Vector2(fromRect.x, fromRect.y);

            return AddTransitionNode(position, transition, from);
        }

        public static TransitionNode AddTransitionNode(Vector2 position, Transition transition, StateNode from)
        {
            TransitionNode transitionNode = CreateInstance<TransitionNode>();
            transitionNode.Init(from, transition);
            transitionNode.windowRect = new Rect(position.x, position.y, 200, 80);
            transitionNode.windowTitle = "Condition Check";
            windows.Add(transitionNode);
            from.dependencies.Add(transitionNode);

            return transitionNode;
        }

        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                (left) ? start.x + start.width : start.x,                
                start.y + (start.height * 0.5f),
                0
            );

            Vector3 endPos = new Vector3(end.x + (end.width * 0.5f), end.y + (end.height * 0.5f), 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 0.5f);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 1);
        }

        public static void ClearWindowsFromList(List<BaseNode> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if(windows.Contains(list[i]))
                {
                    windows.Remove(list[i]); 
                }
            }
        }

        public static void LoadGraph()
        {
            windows.Clear();
            windows.Add(graphNode);
            currentGraph.Init();

            if (currentGraph == null) { return; }

            List<SavedStateNode> savedNodes = new List<SavedStateNode>();
            savedNodes.AddRange(currentGraph.savedStateNodes);
            currentGraph.savedStateNodes.Clear();

            for (int i = savedNodes.Count - 1; i >= 0; i--)
            {
                StateNode node = AddStateNode(savedNodes[i].position);
                node.currentState = savedNodes[i].state;
                node.collapse = savedNodes[i].isCollapsed;
                currentGraph.SetStateNode(node);

                //load transitions
            }
        }
    }
}