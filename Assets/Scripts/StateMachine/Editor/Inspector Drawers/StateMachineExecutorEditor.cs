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

        private StateMachineData initialData;

        private void Awake()
        {
            executor = (StateMachineExecutor)target;
            initialData = executor.StateMachineData; 
        }

        protected void OnEnable()
        {
            editorUI = new StateMachineUIImplementation(initialData, executor.StateMachineLogic, Repaint);
            editorUI.OnEnable();
        }

        protected void OnDisable()
        {
            editorUI.OnDisable();
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