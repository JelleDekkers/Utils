using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class PreviewAttribute : PropertyAttribute
{
    public readonly int Height;

    public PreviewAttribute(int height)
    {
        Height = height;
    }
}
