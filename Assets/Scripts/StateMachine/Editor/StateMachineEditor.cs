using UnityEditor;

namespace StateMachine
{
    /// <summary>
    /// Editor class for <see cref="StateMachine"/>
    /// </summary>
    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : Editor
    {
        private StateMachineRenderer renderer;

        protected void OnEnable()
        {
            renderer = new StateMachineRenderer((StateMachine)target, Repaint);
        }

        public override void OnInspectorGUI()
        {
            renderer.OnInspectorGUI();
        }
    }
}