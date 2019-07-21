using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Editor class for <see cref="StateMachineExecutor"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineExecutor))]
    public class StateMachineExecutorEditor : Editor
    {
        private StateMachineExecutor executor;
        private StateMachineEditorManager manager;
        private StateMachineData stateMachineData;

        protected void OnEnable()
        {
            executor = (StateMachineExecutor)target;
            stateMachineData = executor.StateMachineData;

            if (stateMachineData != null)
            {
                manager = new StateMachineEditorManager(executor.StateMachineData, Repaint, executor);
            }
        }

        public override void OnInspectorGUI()
        {
            Repaint();

            base.OnInspectorGUI();

            if (stateMachineData != null)
            {
                EditorGUILayout.Space();
                manager.OnInspectorGUI();
            }

            if(executor.StateMachineData != stateMachineData)
            {
                stateMachineData = executor.StateMachineData;

                if(stateMachineData != null)
                {
                    manager = new StateMachineEditorManager(executor.StateMachineData, Repaint);
                }
                else
                {
                    manager = null;
                }
            }
        }
    }
}