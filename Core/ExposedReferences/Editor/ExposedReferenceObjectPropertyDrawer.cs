using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    [CustomPropertyDrawer(typeof(ExposedObjectReference<>))]
    public class ExposedReferenceObjectPropertyDrawer : PropertyDrawer
    {
        private ExposedReferencesTable exposedReferencesTable;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (exposedReferencesTable == null)
                exposedReferencesTable = Object.FindObjectOfType<ExposedReferencesTable>();

            if (exposedReferencesTable != null)
            {
                SerializedObject serializedObject = new SerializedObject(property.serializedObject.targetObject, exposedReferencesTable);
                SerializedProperty referenceProperty = serializedObject.FindProperty(property.propertyPath).FindPropertyRelative("reference");

                // This is needed to fix a strange bug where the object is null when adding an element to the list
                try
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(position, referenceProperty, label, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(exposedReferencesTable);
                    }
                }
                catch (System.Exception)
                {
                    referenceProperty.FindPropertyRelative("exposedName").stringValue = "";
                }
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.LabelField(position, label.text, "No ExposedReferenceTable found in scene!");
                GUI.enabled = true;
            }
        }
    }
}