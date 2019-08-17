using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineData))]
    public class StateMachineDataEditor : Editor
    {
        private StateMachineUIImplementation editorUI;

        protected void OnEnable()
        {
            editorUI = new StateMachineUIImplementation(target as StateMachineData, null, Repaint);
            editorUI.OnEnable();
        }

        protected void OnDisable()
        {
            editorUI.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            editorUI.OnInspectorGUI();
        }

        private void OnDestroy()
        {
            editorUI?.Dispose();
        }
    }
}