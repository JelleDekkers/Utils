using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    // TODO: make a more generic DRY version 

	[CustomPropertyDrawer(typeof(ConstantTypeReferenceAttribute))]
	public class ConstantTypeReferenceAttributeDrawer : PropertyDrawer
	{
		private ConstantTypeReferenceAttribute constAttribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			constAttribute = attribute as ConstantTypeReferenceAttribute;

            if (property.type == "ushort")
            {
                DrawUshortDropdown(position, property, label);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
			{
				DrawIntDropdown(position, property, label);
			}
			else if(property.propertyType == SerializedPropertyType.String)
			{
				DrawStringDropDown(position, property, label);
			}
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                DrawFloatingPointDropdown(position, property, label);
            }
            else
			{
				Debug.LogError("ConstantTypeReferenceAttributeDrawer: no support for " + property.type + " type, create a new function that is able to draw it.");
			}
		}

        private void DrawIntDropdown(Rect position, SerializedProperty property, GUIContent label)
        {
            int currentConstant = property.intValue;
            List<int> allConstants = GetAllConstantOfType<int>(constAttribute, out List<string> constNames);

            int selectedIndex = allConstants.IndexOf(currentConstant);

            EditorGUI.BeginProperty(position, label, property);

            if (allConstants.Count == 0)
            {
                EditorGUI.LabelField(position, "No constants found of type int");
            }
            else
            {
                if (selectedIndex < 0)
                    selectedIndex = 0;

                int index = EditorGUI.Popup(position, property.displayName, selectedIndex, constNames.ToArray());
                property.intValue = allConstants[index];
            }

			EditorGUI.EndProperty();
		}

        private void DrawUshortDropdown(Rect position, SerializedProperty property, GUIContent label)
        {
            ushort currentConstant = (ushort)property.intValue;
            List<ushort> allConstants = GetAllConstantOfType<ushort>(constAttribute, out List<string> constNames);
            int selectedIndex = allConstants.IndexOf(currentConstant);

            EditorGUI.BeginProperty(position, label, property);

            if (allConstants.Count == 0)
            {
                EditorGUI.LabelField(position, "No constants found of type ushort");
            }
            else
            {
                if (selectedIndex < 0)
                    selectedIndex = 0;

                int index = EditorGUI.Popup(position, property.displayName, selectedIndex, constNames.ToArray());
                property.intValue = allConstants[index];
            }

            EditorGUI.EndProperty();
        }

        private void DrawStringDropDown(Rect position, SerializedProperty property, GUIContent label)
		{
            string currentConstant = property.stringValue;

            List<string> allConstants = GetAllConstantOfType<string>(constAttribute, out List<string> constNames);
            int selectedIndex = allConstants.IndexOf(currentConstant);

            EditorGUI.BeginProperty(position, label, property);

            if (allConstants.Count == 0)
            {
                EditorGUI.LabelField(position, "No constants found of type string");
            }
            else
            {
                if (selectedIndex < 0)
                    selectedIndex = 0;

                int index = EditorGUI.Popup(position, property.displayName, selectedIndex, constNames.ToArray());
                property.stringValue = allConstants[index];
            }

            EditorGUI.EndProperty();
        }

        private void DrawFloatingPointDropdown(Rect position, SerializedProperty property, GUIContent label)
        {
            float currentConstant = property.floatValue;
            List<float> allConstants = GetAllConstantOfType<float>(constAttribute, out List<string> constNames);
            int selectedIndex = allConstants.IndexOf(currentConstant);

            EditorGUI.BeginProperty(position, label, property);

            if (allConstants.Count == 0)
            {
                EditorGUI.LabelField(position, "No constants found of type float");
            }
            else
            {
                if (selectedIndex < 0)
                    selectedIndex = 0;

                int index = EditorGUI.Popup(position, property.displayName, selectedIndex, constNames.ToArray());
                property.floatValue = allConstants[index];
            }

            EditorGUI.EndProperty();
        }

        private List<T> GetAllConstantOfType<T>(ConstantTypeReferenceAttribute attribute, out List<string> constNames, bool includeParentClasses = false)
		{
			List<T> foundConstFields = new List<T>();
			constNames = new List<string>();
			FieldInfo[] allFields = attribute.type.GetFields(BindingFlags.Public | BindingFlags.Static); // if includeParentClasses: | BindingFlags.FlattenHierarchy

            foreach (FieldInfo field in allFields)
			{
				if(field.FieldType == typeof(T) && field.IsLiteral)
				{
					T constField = (T)field.GetRawConstantValue();
					if(!foundConstFields.Contains(constField))
					{
						foundConstFields.Add(constField);
						constNames.Add(field.Name);
					}
				}
			}

			return foundConstFields;
		}
	}
}
