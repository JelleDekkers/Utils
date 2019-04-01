using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Editor class for <see cref="StateMachineActivator"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineActivator))]
    public class StateMachineActivatorEditor : Editor
    {
        private StateMachineActivator activator;
        private StateMachineRenderer renderer;

        private StateMachine stateMachine;

        protected void OnEnable()
        {
            activator = (StateMachineActivator)target;
            stateMachine = activator.StateMachine;

            if (stateMachine != null)
            {
                renderer = new StateMachineRenderer(activator.StateMachine, Repaint);
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

            if(activator.StateMachine != stateMachine)
            {
                stateMachine = activator.StateMachine;

                if(stateMachine != null)
                {
                    renderer = new StateMachineRenderer(activator.StateMachine, Repaint);
                }
                else
                {
                    renderer = null;
                }
            }
        }
    }
}