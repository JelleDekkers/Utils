using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Attributes
{
    public static class AttachAttributesUtils
    {
        public static string GetPropertyType(this SerializedProperty property)
        {
            var type = property.type;
            var match = Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            return type;
        }

        public static Type StringToType(this string aClassName) => System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.IsSubclassOf(typeof(Component)) && x.Name == aClassName);
    }

    /// Base class for Attach Attribute
    public abstract class AttachAttributePropertyDrawer : PropertyDrawer
    {
        private readonly Color GUIColorDefault = new Color(.6f, .6f, .6f, 1);
        private readonly Color GUIColorNull = new Color(1f, .5f, .5f, 1);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isPropertyValueNull = property.objectReferenceValue == null;

            var prevColor = GUI.color;
            GUI.color = isPropertyValueNull ? GUIColorNull : GUIColorDefault;

            EditorGUI.PropertyField(position, property, label, true);

            if (isPropertyValueNull)
            {
                var type = property.GetPropertyType().StringToType();
                var go = ((MonoBehaviour)(property.serializedObject.targetObject)).gameObject;
                UpdateProperty(property, go, type);
            }

            property.serializedObject.ApplyModifiedProperties();
            GUI.color = prevColor;
        }

        public abstract void UpdateProperty(SerializedProperty property, GameObject go, Type type);
    }

    #region Attribute Editors

    /// GetComponent
    [CustomPropertyDrawer(typeof(GetComponentAttribute))]
    public class GetComponentAttributeEditor : AttachAttributePropertyDrawer
    {
        public override void UpdateProperty(SerializedProperty property, GameObject go, Type type)
        {
            property.objectReferenceValue = go.GetComponent(type);
        }
    }

    /// GetComponentInChildren
    [CustomPropertyDrawer(typeof(GetComponentInChildrenAttribute))]
    public class GetComponentInChildrenAttributeEditor : AttachAttributePropertyDrawer
    {
        public override void UpdateProperty(SerializedProperty property, GameObject go, Type type)
        {
            GetComponentInChildrenAttribute labelAttribute = (GetComponentInChildrenAttribute)attribute;
            property.objectReferenceValue = go.GetComponentInChildren(type, labelAttribute.IncludeInactive);
        }
    }

    /// AddComponent
    [CustomPropertyDrawer(typeof(AddComponentAttribute))]
    public class AddComponentAttributeEditor : AttachAttributePropertyDrawer
    {
        public override void UpdateProperty(SerializedProperty property, GameObject go, Type type)
        {
            property.objectReferenceValue = go.AddComponent(type);
        }
    }

    /// FindObjectOfType
    [CustomPropertyDrawer(typeof(FindObjectOfTypeAttribute))]
    public class FindObjectOfTypeAttributeEditor : AttachAttributePropertyDrawer
    {
        public override void UpdateProperty(SerializedProperty property, GameObject go, Type type)
        {
            property.objectReferenceValue = FindObjectsOfTypeByName(property.GetPropertyType());
        }

        public UnityEngine.Object FindObjectsOfTypeByName(string aClassName)
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var types = assemblies[i].GetTypes();
                for (int n = 0; n < types.Length; n++)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(types[n]) && aClassName == types[n].Name)
                        return UnityEngine.Object.FindObjectOfType(types[n]);
                }
            }
            return new UnityEngine.Object();
        }
    }
    #endregion
}