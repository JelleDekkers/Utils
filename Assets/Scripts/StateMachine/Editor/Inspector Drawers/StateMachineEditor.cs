using UnityEditor;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineData))]
    public class StateMachineEditor : Editor
    {
        private StateMachineEditorManager manager;

        protected void OnEnable()
        {
            manager = new StateMachineEditorManager((StateMachineData)target, Repaint);
        }

        public override void OnInspectorGUI()
        {
            manager.OnInspectorGUI();
        }

        private void OnDestroy()
        {
            manager.Dispose();
        }
    }
}