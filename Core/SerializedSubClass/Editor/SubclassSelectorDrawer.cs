#if UNITY_2019_3_OR_NEWER
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Utils.Core.SerializedSubClass
{
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        struct TypePopupCache
        {
            public AdvancedTypePopup TypePopup { get; }
            public AdvancedDropdownState State { get; }
            public TypePopupCache(AdvancedTypePopup typePopup, AdvancedDropdownState state)
            {
                TypePopup = typePopup;
                State = state;
            }
        }

        private const int MaxTypePopupLineCount = 13;
        private static readonly Type UnityObjectType = typeof(UnityEngine.Object);
        private static readonly GUIContent NullDisplayName = new GUIContent(TypeMenuUtility.NullDisplayName);
        private static readonly GUIContent IsNotManagedReferenceLabel = new GUIContent("The property type is not manage reference.");

        private readonly Dictionary<string, TypePopupCache> typePopups = new Dictionary<string, TypePopupCache>();
        private readonly Dictionary<string, GUIContent> typeNameCaches = new Dictionary<string, GUIContent>();

        private SerializedProperty targetProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2 offsetPos = position.position;
            offsetPos.x += 15f; // hardcoded magic number. seems consistent across unity versions as unity itself does not expose any value for this
            position.position = offsetPos;
            position.width -= 15f;

            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                // Render label first to avoid label overlap for lists
                Rect foldoutLabelRect = new Rect(position);

                foldoutLabelRect.height = EditorGUIUtility.singleLineHeight;
                foldoutLabelRect = EditorGUI.IndentedRect(foldoutLabelRect);
                Rect popupPosition = EditorGUI.PrefixLabel(foldoutLabelRect, label);

                // Draw the subclass selector popup.
                if (EditorGUI.DropdownButton(popupPosition, GetTypeName(property), FocusType.Keyboard))
                {
                    TypePopupCache popup = GetTypePopup(property);
                    targetProperty = property;
                    popup.TypePopup.Show(popupPosition);
                }

                // Draw the foldout.
                if (!string.IsNullOrEmpty(property.managedReferenceFullTypename))
                {
                    Rect foldoutRect = new Rect(position);
                    foldoutRect.height = EditorGUIUtility.singleLineHeight;
                    foldoutRect.x -= 12;
                    property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true);
                }

                // Draw property if expanded.
                if (property.isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        // Check if a custom property drawer exists for this type.
                        PropertyDrawer customDrawer = GetCustomPropertyDrawer(property);
                        if (customDrawer != null)
                        {
                            // Draw the property with custom property drawer.
                            Rect indentedRect = position;
                            float foldoutDifference = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            indentedRect.height = customDrawer.GetPropertyHeight(property, label);
                            indentedRect.y += foldoutDifference;
                            customDrawer.OnGUI(indentedRect, property, label);
                        }
                        else
                        {
                            // Draw the properties of the child elements.
                            // NOTE: In the following code, since the foldout layout isn't working properly, I'll iterate through the properties of the child elements myself.
                            // EditorGUI.PropertyField(position, property, GUIContent.none, true);

                            Rect childPosition = position;
                            childPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            foreach (SerializedProperty childProperty in property.GetChildProperties())
                            {
                                float height = EditorGUI.GetPropertyHeight(childProperty, new GUIContent(childProperty.displayName, childProperty.tooltip), true);
                                childPosition.height = height;
                                EditorGUI.PropertyField(childPosition, childProperty, true);

                                childPosition.y += height + EditorGUIUtility.standardVerticalSpacing;
                            }
                        }
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label, IsNotManagedReferenceLabel);
            }

            EditorGUI.EndProperty();
        }

        private PropertyDrawer GetCustomPropertyDrawer(SerializedProperty property)
        {
            Type propertyType = ManagedReferenceUtility.GetType(property.managedReferenceFullTypename);
            if (propertyType != null && PropertyDrawerCache.TryGetPropertyDrawer(propertyType, out PropertyDrawer drawer))
            {
                return drawer;
            }
            return null;
        }

        private TypePopupCache GetTypePopup(SerializedProperty property)
        {
            // Cache this string. This property internally call Assembly.GetName, which result in a large allocation.
            string managedReferenceFieldTypename = property.managedReferenceFieldTypename;

            if (!typePopups.TryGetValue(managedReferenceFieldTypename, out TypePopupCache result))
            {
                var state = new AdvancedDropdownState();

                Type baseType = ManagedReferenceUtility.GetType(managedReferenceFieldTypename);
                var popup = new AdvancedTypePopup(
                    TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(p =>
                        (p.IsPublic || p.IsNestedPublic) &&
                        !p.IsAbstract &&
                        !p.IsGenericType &&
                        !UnityObjectType.IsAssignableFrom(p) &&
                        Attribute.IsDefined(p, typeof(SerializableAttribute))
                    ),
                    MaxTypePopupLineCount,
                    state,
                    "Select Type",
                    true
                );
                popup.OnItemSelected += item =>
                {
                    Type type = item.Type;

                    // Apply changes to individual serialized objects.
                    foreach (var targetObject in targetProperty.serializedObject.targetObjects)
                    {
                        SerializedObject individualObject = new SerializedObject(targetObject);
                        SerializedProperty individualProperty = individualObject.FindProperty(targetProperty.propertyPath);
                        object obj = individualProperty.SetManagedReference(type);
                        individualProperty.isExpanded = (obj != null);

                        individualObject.ApplyModifiedProperties();
                        individualObject.Update();
                    }
                };

                result = new TypePopupCache(popup, state);
                typePopups.Add(managedReferenceFieldTypename, result);
            }
            return result;
        }

        private GUIContent GetTypeName(SerializedProperty property)
        {
            // Cache this string.
            string managedReferenceFullTypename = property.managedReferenceFullTypename;

            if (string.IsNullOrEmpty(managedReferenceFullTypename))
            {
                return NullDisplayName;
            }
            if (typeNameCaches.TryGetValue(managedReferenceFullTypename, out GUIContent cachedTypeName))
            {
                return cachedTypeName;
            }

            Type type = ManagedReferenceUtility.GetType(managedReferenceFullTypename);
            string typeName = null;

            SubClassMenuAttribute typeMenu = TypeMenuUtility.GetAttribute(type);
            if (typeMenu != null)
            {
                typeName = typeMenu.GetTypeNameWithoutPath();
                if (!string.IsNullOrWhiteSpace(typeName))
                {
                    typeName = ObjectNames.NicifyVariableName(typeName);
                }
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                typeName = ObjectNames.NicifyVariableName(type.Name);
            }

            GUIContent result = new GUIContent(typeName);
            typeNameCaches.Add(managedReferenceFullTypename, result);
            return result;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PropertyDrawer customDrawer = GetCustomPropertyDrawer(property);
            if (customDrawer != null)
            {
                return property.isExpanded ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + customDrawer.GetPropertyHeight(property, label) : EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return property.isExpanded ? EditorGUI.GetPropertyHeight(property, true) : EditorGUIUtility.singleLineHeight;
            }
        }

    }
}
#endif