using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow.Inspector
{
    public class InspectorUIFallbackBehaviour : IInspectorUIBehaviour
    {
        protected StateMachineEditorManager Manager { get; private set; }
        protected SerializedObject Target { get; private set; }

        public InspectorUIFallbackBehaviour(StateMachineEditorManager manager, SerializedObject target)
        {
            Manager = manager;
            Target = target;
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawInspectorContent(e);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawInspectorContent(Event e)
        {
            InspectorUIUtility.DrawHeader(Target.targetObject.name);
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawAllProperties(Target);
        }

        public void Refresh() { }
    }
}
