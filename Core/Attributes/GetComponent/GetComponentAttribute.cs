using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    public class AttachPropertyAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentAttribute : AttachPropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class GetComponentInChildrenAttribute : AttachPropertyAttribute
    {
        public bool IncludeInactive { get; private set; }

        public GetComponentInChildrenAttribute(bool includeInactive = false)
        {
            IncludeInactive = includeInactive;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AddComponentAttribute : AttachPropertyAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public class FindObjectOfTypeAttribute : AttachPropertyAttribute { }
}