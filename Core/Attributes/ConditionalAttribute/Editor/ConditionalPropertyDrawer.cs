using UnityEngine;
using UnityEditor;
 
public abstract class ConditionalPropertyDrawer<T> : PropertyDrawer where T : ConditionalBaseAttribute
{
    protected T conditionalAttribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (conditionalAttribute == null)
            conditionalAttribute = (T)attribute;
    }

    public virtual bool GetConditionResult(SerializedProperty property)
    {
        if (conditionalAttribute == null)
            conditionalAttribute = (T)attribute;

        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, conditionalAttribute.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + conditionalAttribute.ConditionalSourceField);
        }

        return enabled;
    }
}