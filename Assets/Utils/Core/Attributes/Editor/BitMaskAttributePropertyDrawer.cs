using UnityEditor;
using UnityEngine;
using Utils.Core.Extensions;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class EnumBitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var typeAttr = attribute as BitMaskAttribute;
            label.text = label.text + "(" + prop.intValue + ")";
            prop.intValue = BitMaskExtension.DrawBitMaskField(position, prop.intValue, typeAttr.propType, label);
        }
    }
}
