﻿using UnityEditor;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineScriptableObjectData"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineScriptableObjectData))]
    public class StateMachineDataEditor : Editor
    {
        private StateMachineUIImplementation editorUI;

        protected void OnEnable()
        {
            editorUI = new StateMachineUIImplementation(target as StateMachineScriptableObjectData, null, Repaint);
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