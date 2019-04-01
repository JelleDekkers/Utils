using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestSO")]
public class SOTest : ScriptableObject
{
    [SerializeField] SOInstance sOInstance;

    public void SetSOInstance(SOInstance so)
    {
        sOInstance = so;
    }
}

