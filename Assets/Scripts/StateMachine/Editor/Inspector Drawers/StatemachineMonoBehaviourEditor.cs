using UnityEditor;

namespace Utils.Core.Flow
{
    [CustomEditor(typeof(StateMachineMonoBehaviour))]
    public class StatemachineMonoBehaviourEditor : Editor
    {
        private StateMachineLayerRenderer layerRenderer;
        private StateMachineMonoBehaviour component;

        private void Awake()
        {
            component = target as StateMachineMonoBehaviour;
            layerRenderer = new StateMachineLayerRenderer(component.Data, Repaint, component.StateMachine);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            layerRenderer.OnInspectorGUI();
        }

        public void OnDestroy()
        {
            layerRenderer?.Dispose();
        }
    }
}