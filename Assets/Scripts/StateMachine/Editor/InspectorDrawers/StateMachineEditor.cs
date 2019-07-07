using UnityEditor;

namespace StateMachine
{
    /// <summary>
    /// Editor class for <see cref="StateMachineData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineData))]
    public class StateMachineEditor : Editor
    {
        private StateMachineEditorManager renderer;

        protected void OnEnable()
        {
            renderer = new StateMachineEditorManager((StateMachineData)target, Repaint);
        }

        public override void OnInspectorGUI()
        {
            renderer.OnInspectorGUI();
        }
    }
}