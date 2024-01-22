using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalShowAttribute))]
public class ConditionalShowPropertyDrawer : ConditionalPropertyDrawer<ConditionalShowAttribute>
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        bool enabled = GetConditionResult(property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        bool enabled = GetConditionResult(property);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
