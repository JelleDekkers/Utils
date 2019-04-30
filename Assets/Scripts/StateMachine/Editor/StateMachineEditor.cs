using UnityEditor;

namespace StateMachine
{
    /// <summary>
    /// Editor class for <see cref="StateMachineData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineData))]
    public class StateMachineEditor : Editor
    {
        private StateMachineRenderer renderer;

        protected void OnEnable()
        {
            renderer = new StateMachineRenderer((StateMachineData)target, Repaint);
        }

        public override void OnInspectorGUI()
        {
            renderer.OnInspectorGUI();
        }
    }
}