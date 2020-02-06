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
        private StateMachineRenderer editorUI;

        private StateMachineScriptableObjectData initialData;

        public override bool RequiresConstantRepaint() { return EditorApplication.isPlaying; }

        private void Awake()
        {
            executor = (StateMachineExecutor)target;
            initialData = executor.StateMachineData; 
        }

        protected void OnEnable()
        {
            editorUI = new StateMachineRenderer(initialData, executor.StateMachine, Repaint);
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