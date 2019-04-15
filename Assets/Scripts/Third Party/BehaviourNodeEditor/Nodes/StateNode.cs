using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NodeEditor
{
    public class StateNode : BaseNode
    {
        public State currentState;
        public List<BaseNode> dependencies = new List<BaseNode>();
        public bool collapse;
        public bool isDuplicate;
        public State previousState;

        private StateNode previousNode;
        private bool previousCollapse;
        private SerializedObject serializedState;
        private ReorderableList onStateList;
        private ReorderableList onEnterList;
        private ReorderableList onExitList;

        public override void DrawWindow()
        {
            if(currentState == null)
            {
                EditorGUILayout.LabelField("Add state to modify:");
            }
            else
            {
                if(!collapse)
                {

                }
                else
                {
                    windowRect.height = 100;
                }

                collapse = EditorGUILayout.Toggle(" ", collapse);
            }

            currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);

            if(previousCollapse != collapse)
            {
                previousCollapse = collapse;
                //BehaviourEditor.currentGraph.SetStateNode(this);
            }

            if(previousState != currentState)
            {
                serializedState = null;

                isDuplicate = BehaviourEditor.currentGraph.IsStateNodeDuplicate(this);
                if (!isDuplicate)
                {
                    BehaviourEditor.currentGraph.SetStateNode(this);
                    previousState = currentState;

                    for (int i = 0; i < currentState.transitions.Count; i++)
                    {
                    }
                }
            }

            if (isDuplicate)
            {
                EditorGUILayout.LabelField("State is duplicate");
                windowRect.height = 100;
                return;
            }

            if(currentState != null)
            {
                if(serializedState == null)
                {
                    serializedState = new SerializedObject(currentState);
                    onStateList = new ReorderableList(serializedState, serializedState.FindProperty("onState"), true, true, true, true);
                    onEnterList = new ReorderableList(serializedState, serializedState.FindProperty("onEnter"), true, true, true, true);
                    onExitList = new ReorderableList(serializedState, serializedState.FindProperty("onExit"), true, true, true, true);
                }

                if(!collapse)
                {
                    serializedState.Update();
                    HandleReordableList(onStateList, "On State");
                    HandleReordableList(onEnterList, "On Enter");
                    HandleReordableList(onExitList, "On Exit");

                    EditorGUILayout.LabelField("");
                    onStateList.DoLayoutList();

                    EditorGUILayout.LabelField("");
                    onEnterList.DoLayoutList();

                    EditorGUILayout.LabelField("");
                    onExitList.DoLayoutList();

                    serializedState.ApplyModifiedProperties();

                    float standard = 300;
                    standard += onStateList.count * 20;
                    standard += onEnterList.count * 20;
                    standard += onExitList.count * 20;
                    windowRect.height = standard;
                }
            }
        }

        private void HandleReordableList(ReorderableList list, string targetName)
        {
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, targetName);
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
        }

        public override void DrawCurve()
        {

        }

        public Transition AddTransition()
        {
            return currentState.AddTransition();
        }

        public void ClearReferences()
        {
            BehaviourEditor.ClearWindowsFromList(dependencies);
            dependencies.Clear();
        }
    }
}