using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for viewing a <see cref="StateMachineEditorManager"/> in an editor window
    /// </summary>
    public class StateMachineWindow : EditorWindow
    {
        private static readonly Vector2 windowMinSize = new Vector2(500, 500);
        
        private StateMachineEditorManager manager;

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
                manager = new StateMachineEditorManager(stateMachine, Repaint);
            }
        }

        private void OnGUI()
        {
            if (manager != null)
            {
                manager.OnInspectorGUI();
            }
            else
            {
                GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.UpperCenter;
                GUIContent content = new GUIContent(string.Format("No {0} or {1} selected", typeof(StateMachineData).Name, typeof(StateMachineExecutor).Name));
                Vector2 size = centeredStyle.CalcSize(content);
                GUI.Label(new Rect(Screen.width / 2 - size.x / 2, Screen.height / 2 - size.y / 2, size.x, size.y), content, centeredStyle);
            }
        }

        private void OnSelectionChange()
        {
            SetupWindow(this);
        }

        private void OnDestroy()
        {
            manager.Dispose();
        }
    }
}