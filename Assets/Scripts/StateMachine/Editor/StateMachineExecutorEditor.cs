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
        private StateMachineExecutor activator;
        private StateMachineEditorManager renderer;

        private StateMachineData stateMachine;

        protected void OnEnable()
        {
            activator = (StateMachineExecutor)target;
            stateMachine = activator.StateMachineData;

            if (stateMachine != null)
            {
                renderer = new StateMachineEditorManager(activator.StateMachineData, Repaint);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (stateMachine != null)
            {
                EditorGUILayout.Space();
                renderer.OnInspectorGUI();
            }

            if(activator.StateMachineData != stateMachine)
            {
                stateMachine = activator.StateMachineData;

                if(stateMachine != null)
                {
                    renderer = new StateMachineEditorManager(activator.StateMachineData, Repaint);
                }
                else
                {
                    renderer = null;
                }
            }
        }
    }
}