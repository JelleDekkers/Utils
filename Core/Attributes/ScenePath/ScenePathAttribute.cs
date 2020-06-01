using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    /// <summary>
    /// Attribute for showing a dropdown of all scenes
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ScenePathAttribute : PropertyAttribute
    {
        public string scene;
    }
}