using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PreviewAttribute : PropertyAttribute
    {
        public readonly int Height;

        public PreviewAttribute(int height)
        {
            Height = height;
        }
    }
}