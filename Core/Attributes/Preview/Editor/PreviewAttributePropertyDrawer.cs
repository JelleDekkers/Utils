using UnityEditor;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewAttributePropertyDrawer : PropertyDrawer
    {
        private Editor prefabPreviewEditor;
        private bool drawPreview;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.serializedObject.targetObject != null)
            {
                Object previewObject = fieldInfo.GetValue(property.serializedObject.targetObject) as Object;

                if (previewObject != null)
                {
                    drawPreview = true;

                    if (prefabPreviewEditor == null)
                    {
                        prefabPreviewEditor = Editor.CreateEditor(previewObject);
                    }

                    rect.y += EditorGUIUtility.singleLineHeight;
                    rect.height = (attribute as PreviewAttribute).Height;

                    prefabPreviewEditor.OnPreviewGUI(rect, EditorStyles.whiteLabel);
                }
                else
                {
                    DestroyEditor();
                }
            }
            else
            {
                DestroyEditor();
            }
        }

        private void DestroyEditor()
        {
            if (prefabPreviewEditor != null)
            {
                Debug.Log("[EDITOR]Destroying Editor");
                Object.DestroyImmediate(prefabPreviewEditor);
                prefabPreviewEditor = null;
            }

            drawPreview = false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return drawPreview ? (attribute as PreviewAttribute).Height + EditorGUIUtility.singleLineHeight : base.GetPropertyHeight(property, label);
        }
    }
}