using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    [CustomPropertyDrawer(typeof(IntMinMax))]
    public class IntMinMaxPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty min, max;
        private string name;
        //private bool cache;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if (!cache) { // uncommented because it seems to interfere with properties shown in the dictionary
            name = property.displayName;
            property.Next(true);
            min = property.Copy();
            property.Next(true);
            max = property.Copy();
            //cache = true;
            //}

            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

            //show the X and Y from the point
            EditorGUIUtility.labelWidth = 25f;
            contentPosition.width *= 0.5f;
            //EditorGUI.indentLevel = 1;

            // Begin/end property & change check make each field
            // behave correctly when multi-object editing.
            EditorGUI.BeginProperty(contentPosition, label, min);
            {
                EditorGUI.BeginChangeCheck();
                int newVal = EditorGUI.IntField(contentPosition, new GUIContent("Min"), min.intValue);
                if (EditorGUI.EndChangeCheck())
                    min.intValue = newVal;
            }
            EditorGUI.EndProperty();

            contentPosition.x += contentPosition.width;

            EditorGUI.BeginProperty(contentPosition, label, max);
            {
                EditorGUI.BeginChangeCheck();
                int newVal = EditorGUI.IntField(contentPosition, new GUIContent("Max"), max.intValue);
                if (EditorGUI.EndChangeCheck())
                    max.intValue = newVal;
            }
            //EditorGUI.indentLevel = 0;
            EditorGUI.EndProperty();
        }
    }
}