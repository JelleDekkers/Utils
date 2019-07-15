using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NodeRenderer<T> where T : ScriptableObject
{
    T DataObject { get; }
}
