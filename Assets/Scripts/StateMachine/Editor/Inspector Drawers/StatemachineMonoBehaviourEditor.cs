using UnityEditor;

namespace Utils.Core.Flow
{
    [CustomEditor(typeof(StateMachineMonoBehaviour))]
    public class StatemachineMonoBehaviourEditor : Editor
    {
        private StateMachineMonoBehaviour stateMachine;
        private StateMachineLayerRenderer layerRenderer;

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
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            layerRenderer.OnInspectorGUI();
        }

        public void OnDisable()
        {
            layerRenderer?.Dispose();
        }
    }
}