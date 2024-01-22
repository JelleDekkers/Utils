using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages <see cref="ExposedObjectReference{T}"/> in asset objects such as <see cref="StateAction"/> that reference scene objects
/// This object is required in the scene if you are working with <see cref="ExposedObjectReference{T}"/>
/// </summary>
[System.Serializable]
public class ExposedReferencesTable : SingletonMonoBehaviour<ExposedReferencesTable>, IExposedPropertyTable
{
    public List<Object> objectReferences = new List<Object>();
    public List<PropertyName> propertyNames = new List<PropertyName>();

    public void ClearReferenceValue(PropertyName id)
    {
        int index = propertyNames.IndexOf(id);
        if (index != -1)
        {
            objectReferences.RemoveAt(index);
            propertyNames.RemoveAt(index);
        }
    }

    public Object GetReferenceValue(PropertyName id, out bool idValid)
    {
        int index = propertyNames.IndexOf(id);
        if (index != -1)
        {
            idValid = true;
            return objectReferences[index];
        }
        
        idValid = false;
        return null;
    }

    public void SetReferenceValue(PropertyName id, Object value)
    {
        int index = propertyNames.IndexOf(id);
        if (index != -1)
        {
            objectReferences[index] = value;
        }
        else
        {
            propertyNames.Add(id);
            objectReferences.Add(value);
        }
    }
}
