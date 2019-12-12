using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
	[CustomPropertyDrawer(typeof(ConstantTypeReferenceAttribute))]
	public class ConstantTagDrawer : PropertyDrawer
	{
		public const int TEST1 = 1;
		public const int TEST2 = 202;

		public const string STRING_TEST_1 = "Test 1";
		public const string STRING_TEST_2 = "Test 2";

		private ConstantTypeReferenceAttribute tag;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			tag = attribute as ConstantTypeReferenceAttribute;

			if(tag.constType == typeof(int))
			{
				DrawIntDropdown(position, property, label);
			}
			else if(tag.constType == typeof(string))
			{
				DrawStringDropDown(position, property, label);
			}
			else
			{
				Debug.LogError("ConstantTagDrawer, no support for " + tag.constType + " type, create a new function that draws it.");
			}
		}

		private void DrawIntDropdown(Rect position, SerializedProperty property, GUIContent label)
		{
			int currentTag = property.intValue;
			List<int> availableTags = GetAllConstantTagsOfType<int>(tag, out List<string> tagNames);
			int selectedIndex = availableTags.IndexOf(currentTag);

			EditorGUI.BeginProperty(position, label, property);

			if (selectedIndex < 0)
				selectedIndex = 0;

			int index = EditorGUI.Popup(position, selectedIndex, tagNames.ToArray());
			property.intValue = availableTags[index];
			EditorGUI.EndProperty();
		}

		private void DrawStringDropDown(Rect position, SerializedProperty property, GUIContent label)
		{

		}

		private List<T> GetAllConstantTagsOfType<T>(ConstantTypeReferenceAttribute attribute, out List<string> tagNames)
		{
			List<T> foundConstFields = new List<T>();
			tagNames = new List<string>();
			FieldInfo[] fields = attribute.constType.GetFields();

			foreach(FieldInfo field in fields)
			{
				if(field.FieldType == attribute.constType && field.IsLiteral)
				{
					T constField = (T)field.GetRawConstantValue();
					if(!foundConstFields.Contains(constField))
					{
						foundConstFields.Add(constField);
						tagNames.Add(constField.ToString());
					}
				}
			}

			return foundConstFields;
		}
	}
}
