using UnityEditor;
using UnityEngine;

namespace Utils.Core.SceneManagement
{
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        private const float BackLabelWidth = 100;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
            SerializedProperty sceneName = property.FindPropertyRelative("sceneName");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.width -= BackLabelWidth;
            EditorGUI.BeginChangeCheck();
            Object sceneField = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

            // Enforce sceneName every frame:
            if (sceneAsset.objectReferenceValue != null && sceneField != null)
                sceneName.stringValue = sceneField.name;
            else
                sceneName.stringValue = "";

            if (EditorGUI.EndChangeCheck())
            {
                sceneAsset.objectReferenceValue = sceneField;
            }

            if (sceneAsset.objectReferenceValue != null)
            {
                position.x += position.width;
                position.width = BackLabelWidth;
                bool isAddedInBuild = SceneFieldHelper.IsSceneAddedToBuild(sceneAsset.objectReferenceValue as SceneAsset);
                string text = isAddedInBuild ? "In build" : "Not in build";
                EditorGUI.LabelField(position, new GUIContent(text, "Shows if this scene is added to the build list and will load when switching scenes"), EditorStyles.miniLabel);
            }
            EditorGUI.EndProperty();
        }
    }
}