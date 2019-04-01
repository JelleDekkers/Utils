using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor
{
    [CreateAssetMenu]
    public class BehaviourGraph : ScriptableObject
    {
        public List<SavedStateNode> savedStateNodes = new List<SavedStateNode>();

        private Dictionary<StateNode, SavedStateNode> stateNodesDict = new Dictionary<StateNode, SavedStateNode>();
        private Dictionary<State, StateNode> stateDict = new Dictionary<State, StateNode>();

        public void Init()
        {
            stateNodesDict.Clear();
            stateDict.Clear();
        }

        public void SetNode(BaseNode node)
        {
            if (node is StateNode)
            {
                SetStateNode((StateNode)node);
                return;
            }

            if(node is TransitionNode)
            {
                //
                return;
            }

            if(node is CommentNode)
            {
                //
                return;
            }
        }

        public bool IsStateNodeDuplicate(StateNode node)
        {
            StateNode previousNode = null;
            stateDict.TryGetValue(node.currentState, out previousNode);

            if (previousNode != null)
            {
                return true;
            }

            return false;
        }

        public void SetStateNode(StateNode node)
        {
            if(node.isDuplicate) { return; }

            if (node.previousState)
            {
                stateDict.Remove(node.previousState);
            }

            if (node.currentState == null)
            {
                return;
            }

            SavedStateNode savedStateNode = GetSavedStateNode(node);
            if(savedStateNode == null)
            {
                savedStateNode = new SavedStateNode();
                savedStateNodes.Add(savedStateNode);
                stateNodesDict.Add(node, savedStateNode);
            }

            savedStateNode.state = node.currentState;
            savedStateNode.position = new Vector2(node.windowRect.x, node.windowRect.y);
            savedStateNode.isCollapsed = node.collapse;
            stateDict.Add(savedStateNode.state, node);
        }

        public void ClearStateNode(StateNode node)
        {
            SavedStateNode savedStateNode = GetSavedStateNode(node);
            if(savedStateNode != null)
            {
                savedStateNodes.Remove(savedStateNode);
                stateNodesDict.Remove(node);

            }
        }

        private SavedStateNode GetSavedStateNode(StateNode node)
        {
            SavedStateNode savedStateNode = null;
            stateNodesDict.TryGetValue(node, out savedStateNode);
            return savedStateNode;
        }

        public StateNode GetStateNode(State node)
        {
            StateNode stateNode = null;
            stateDict.TryGetValue(node, out stateNode);
            return stateNode;
        }
    }

    [System.Serializable]
    public class SavedStateNode
    {
        public State state;
        public Vector2 position;
        public bool isCollapsed;
    }

    [System.Serializable]
    public class SavedTransition
    {


    }
}