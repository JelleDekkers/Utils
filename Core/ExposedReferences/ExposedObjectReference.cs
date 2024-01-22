using UnityEngine;

[System.Serializable]
public sealed class ExposedObjectReference<T> where T : Object
{
    public bool HasValue => reference.exposedName != null;
    public ExposedReference<T> reference;
}
