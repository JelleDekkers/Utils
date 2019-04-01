using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSO")]
public class SOTest : ScriptableObject
{
    [SerializeField] List<SOInstance> sOInstances;

    public void AddSoInstance(SOInstance so)
    {
        sOInstances.Add(so);
    }

    public void RemoveSoInstance()
    {
        sOInstances.RemoveAt(sOInstances.Count - 1);
    }
}

