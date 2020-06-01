using UnityEditor;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(ScenePathAttribute))]
    public class ScenePathAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, property.name, "Needs to be of type string!");
                return;
            }

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            EditorGUI.BeginChangeCheck();

            SceneAsset oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            SceneAsset newScene = EditorGUI.ObjectField(position, property.name, oldScene, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                property.stringValue = newPath;
            }

            EditorGUI.EndProperty();
        }
    }
}