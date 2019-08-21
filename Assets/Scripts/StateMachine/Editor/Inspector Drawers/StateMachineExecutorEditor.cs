using UnityEditor;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineExecutor"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineExecutor))]
    public class StateMachineExecutorEditor : Editor
    {
        private StateMachineExecutor executor;
        private StateMachineUIImplementation editorUI;

        private StateMachineScriptableObjectData initialData;

        private void Awake()
        {
            executor = (StateMachineExecutor)target;
            initialData = executor.StateMachineData; 

            Undo.undoRedoPerformed += Refresh;
        }

        protected void OnEnable()
        {
            editorUI = new StateMachineUIImplementation(initialData, executor.StateMachine, Repaint);
            editorUI.OnEnable();

            Undo.undoRedoPerformed -= Refresh;
        }

        protected void OnDisable()
        {
            editorUI.OnDisable();
        }

        private void Refresh()
        {
            editorUI.Refresh();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            editorUI.OnInspectorGUI(); 

            if(executor.StateMachineData != initialData)
            {
                editorUI.OnStateMachineDataChanged(executor.StateMachineData);
                initialData = executor.StateMachineData;
            }
        }

        private void OnDestroy()
        {
            editorUI?.Dispose();
        }
    }
}