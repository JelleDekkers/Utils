using System;
using UnityEngine;

namespace Utils.Core.Attributes
{
    public class BitMaskAttribute : PropertyAttribute
    {
        public Type propType;
        public BitMaskAttribute(Type aType)
        {
            propType = aType;
        }
    }
}