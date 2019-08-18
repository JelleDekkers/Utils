using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for viewing a <see cref="StateMachineLayerRenderer"/> in an editor window
    /// </summary>
    public class StateMachineWindow : EditorWindow
    {
        private static readonly Vector2 windowMinSize = new Vector2(500, 500);
        
        private StateMachineScriptableObjectData data;
        private StateMachineUIImplementation editorUI;

        [MenuItem("Window/State Machine Window")]
        public static StateMachineWindow Init()
        {
            StateMachineWindow window = GetWindow<StateMachineWindow>(false, "State Machine");
            SetupWindow(window);
            return window;
        }

        protected void OnEnable()
        {
            SetupWindow(this);
        }

        protected void OnDisable()
        {
            editorUI?.OnDisable();
        }

        private static void SetupWindow(StateMachineWindow window)
        {
            window.minSize = windowMinSize;

            if (Selection.activeObject is StateMachineScriptableObjectData)
            {
                window.SetTarget(Selection.activeObject as StateMachineScriptableObjectData);
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

        public void SetTarget(StateMachineScriptableObjectData data)
        {
            if(data != null)
            {
                this.data = data;
                editorUI = new StateMachineUIImplementation(data, null, Repaint);
                editorUI.OnEnable();
            }
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (editorUI != null)
            {
                editorUI.OnInspectorGUI();
            }
            else
            {
                GUILayout.Label(string.Format("No {0} or {1} selected", typeof(StateMachineScriptableObjectData).Name, typeof(StateMachineExecutor).Name));
            }
        }

        private void OnSelectionChange()
        {
            SetupWindow(this);
        }

        private void OnDestroy()
        {
            editorUI?.Dispose();
        }
    }
}