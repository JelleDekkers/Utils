using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    /// <summary>
    /// Shows a dropdown of all parameters of an AnimatorController on the same GameObject
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AnimatorParameterAttribute : PropertyAttribute
    {

    }
}