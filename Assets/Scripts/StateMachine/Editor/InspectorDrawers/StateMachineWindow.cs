using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Editor class for viewing a <see cref="StateMachineEditorManager"/> in an editor window
    /// </summary>
    public class StateMachineWindow : EditorWindow
    {
        private static readonly Vector2 windowMinSize = new Vector2(500, 500);
        
        private StateMachineEditorManager renderer;
        private StateMachineData stateMachine;

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

            if (Selection.activeObject is StateMachineData)
            {
                window.SetTarget(Selection.activeObject as StateMachineData);
            }
            if (Selection.activeObject is GameObject)
            {
                var go = Selection.activeObject as GameObject;
                StateMachineExecutor component = go.GetComponent<StateMachineExecutor>();
                if (component != null)
                {
                    window.SetTarget(component.StateMachineData);
                }
            }
        }

        public void SetTarget(StateMachineData stateMachine)
        {
            if(stateMachine != null)
            {
                renderer = new StateMachineEditorManager(stateMachine, Repaint);
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