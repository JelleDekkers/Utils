using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    [CustomPropertyDrawer(typeof(Layer))]
    public class LayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty layerIndex = property.FindPropertyRelative("m_LayerIndex");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(position, layerIndex.intValue);
            }
            EditorGUI.EndProperty();
        }
    }
}