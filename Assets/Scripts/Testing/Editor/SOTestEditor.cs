using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOTest))]
public class SOTestEditor : Editor
{
    private SOTest obj;

    private void OnEnable()
    {
        obj = (SOTest)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Set SO"))
        {
            obj.AddSoInstance(new SOInstance());
        }

        if (GUILayout.Button("Remove instance"))
        {
            obj.RemoveSoInstance();
        }
    }
}
