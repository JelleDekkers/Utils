using UnityEditor;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(ConditionalReadOnlyAttribute))]
    public class ConditionalReadonlyPropertyDrawer : ConditionalPropertyDrawer<ConditionalReadOnlyAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            bool enabled = GetConditionResult(property);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }
    }
}
