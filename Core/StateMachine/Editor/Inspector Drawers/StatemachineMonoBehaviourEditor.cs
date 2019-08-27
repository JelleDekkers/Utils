using UnityEditor;

namespace Utils.Core.Flow
{
    [CustomEditor(typeof(StateMachineMonoBehaviour))]
    public class StatemachineMonoBehaviourEditor : Editor
    {
        private StateMachineMonoBehaviour stateMachine;
        private StateMachineLayerRenderer layerRenderer;

        public override bool RequiresConstantRepaint() { return EditorApplication.isPlaying; }

        private void Awake()
        {
            stateMachine = target as StateMachineMonoBehaviour;

            if(stateMachine.Data == null)
            {
                stateMachine.Data = new StateMachineMonoBehaviourData(stateMachine);
            }
        }

        private void OnEnable()
        {
            layerRenderer = new StateMachineLayerRenderer(stateMachine.Data, Repaint, stateMachine.StateMachine);
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public void OnDisable()
        {
            layerRenderer?.OnDestroy();
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void Refresh()
        {
            layerRenderer.Refresh();
        }

        private void OnUndoRedoPerformed()
        {
            if (Undo.GetCurrentGroupName() != StateMachineEditorUtility.UNDO_INSPECTOR_COMMAND_NAME)
            {
                Refresh();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            layerRenderer.OnInspectorGUI();
        }
    }
}