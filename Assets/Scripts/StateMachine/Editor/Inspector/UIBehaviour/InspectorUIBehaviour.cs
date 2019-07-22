using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class InspectorUIBehaviour
    {
        protected StateMachineEditorManager Manager { get; private set; }
        protected ScriptableObject TargetObject { get; private set; }
        protected SerializedObject SerializedObject { get; private set; }

        public InspectorUIBehaviour(StateMachineEditorManager manager, ScriptableObject target)
        {
            Manager = manager;
            TargetObject = target;

            Refresh();
        }

        public void Refresh()
        {
            SerializedObject = new SerializedObject(TargetObject);
        }

        public void OnInspectorGUI(Event e)
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            DrawInspectorContent(e);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawInspectorContent(Event e)
        {
            InspectorUIUtility.DrawHeader(TargetObject.ToString());
            InspectorUIUtility.DrawHorizontalLine();
            InspectorUIUtility.DrawAllProperties(TargetObject);
        }
    }
}
