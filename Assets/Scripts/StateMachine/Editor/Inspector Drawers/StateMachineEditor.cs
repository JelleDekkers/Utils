using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineData))]
    public class StateMachineEditor : Editor
    {
        private StateMachineUIImplementation editorUI;

        protected void OnEnable()
        {
            editorUI = new StateMachineUIImplementation((StateMachineData)target, Repaint);
        }

        public override void OnInspectorGUI()
        {
            editorUI.OnInspectorGUI();
        }

        private void OnDestroy()
        {
            editorUI.Dispose();
        }
    }
}