using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    [CustomPropertyDrawer(typeof(FloatMinMax))]
    public class FloatMinMaxPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty min, max;
        private string name;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            name = property.displayName;
            property.Next(true);
            min = property.Copy();
            property.Next(true);
            max = property.Copy();

            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

            EditorGUIUtility.labelWidth = 25f;
            contentPosition.width *= 0.5f;

            EditorGUI.BeginProperty(contentPosition, label, min);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("Min"), min.floatValue);
                if (EditorGUI.EndChangeCheck())
                    min.floatValue = newVal;
            }
            EditorGUI.EndProperty();

            contentPosition.x += contentPosition.width;

            EditorGUI.BeginProperty(contentPosition, label, max);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("Max"), max.floatValue);
                if (EditorGUI.EndChangeCheck())
                    max.floatValue = newVal;
            }
            EditorGUI.EndProperty();
        }
    }
}