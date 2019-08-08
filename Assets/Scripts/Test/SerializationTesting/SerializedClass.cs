using System;
using UnityEngine;

[Serializable]
public class SerializedClass<T> where T : class
{
    public Type DataType
    {
        get
        {
            return Type.GetType(dataType);
        }
    }
    [SerializeField] protected string dataType;

    public string Data => data;
    [SerializeField] protected string data;

    public string Identifier => identifier;
    [SerializeField] protected string identifier;

    public SerializedClass(T obj)
    {
        dataType = obj.GetType().ToString();
        data = JsonUtility.ToJson(obj);
        identifier = Guid.NewGuid().ToString();
    }

    public T Instantiate()
    {
        return (T)JsonUtility.FromJson(data, DataType);
    }
}