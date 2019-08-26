using UnityEngine;
using UnityEditor;

namespace Utils.Core.Attributes
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                MinMaxSliderAttribute minMax = attribute as MinMaxSliderAttribute;

                Rect totalValueRect = EditorGUI.PrefixLabel(position, label);
                Rect leftRect = new Rect(totalValueRect.x, totalValueRect.y, 50, totalValueRect.height);
                Rect valueRect = new Rect(leftRect.xMax, totalValueRect.y, totalValueRect.width - leftRect.width * 2 - 4, totalValueRect.height);
                Rect rightRect = new Rect(totalValueRect.xMax - leftRect.width - 2, totalValueRect.y, leftRect.width, totalValueRect.height);

                float minValue = property.vector2Value.x;
                float maxValue = property.vector2Value.y;

                EditorGUI.MinMaxSlider(valueRect, ref minValue, ref maxValue, minMax.minLimit, minMax.maxLimit);

                property.vector2Value = new Vector2(minValue, maxValue);

                leftRect.width -= 5;
                EditorGUI.FloatField(leftRect, minValue);

                rightRect.width -= 5;
                rightRect.x += 5;
                EditorGUI.FloatField(rightRect, maxValue);
            }
            else
            {
                GUI.Label(position, "You can use MinMax only on a Vector2!");
            }
        }
    }
}