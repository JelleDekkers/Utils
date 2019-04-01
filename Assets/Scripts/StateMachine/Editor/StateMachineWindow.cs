using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class StateMachineWindow : EditorWindow
    {
        private static readonly Vector2 windowMinSize = new Vector2(500, 500);
        
        private StateMachineRenderer renderer;
        private StateMachine stateMachine;

        [MenuItem("Window/State Machine")]
        public static StateMachineWindow Init()
        {
            StateMachineWindow window = GetWindow<StateMachineWindow>(false, "State Machine");
            SetupWindow(window);
            return window;
        }

        private static void SetupWindow(StateMachineWindow window)
        {
            window.minSize = windowMinSize;

            if (Selection.activeObject is StateMachine)
            {
                window.SetTarget(Selection.activeObject as StateMachine);
            }
            if (Selection.activeObject is GameObject)
            {
                var go = Selection.activeObject as GameObject;
                StateMachineActivator component = go.GetComponent<StateMachineActivator>();
                if (component != null)
                {
                    window.SetTarget(component.StateMachine);
                }
            }
        }

        public void SetTarget(StateMachine stateMachine)
        {
            if(stateMachine != null)
            {
                renderer = new StateMachineRenderer(stateMachine, Repaint);
            }
        }

        private void OnGUI()
        {
            if (renderer != null)
            {
                renderer.OnInspectorGUI();
            }
            else
            {
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.UpperCenter;
                GUI.Label(new Rect(Screen.width / 2 - 325, Screen.height / 2 - 25, 600, 50), "No State Machine or StateMachineActivator selected", centeredStyle);
            }
        }

        private void OnSelectionChange()
        {
            SetupWindow(this);
        }
    }
}