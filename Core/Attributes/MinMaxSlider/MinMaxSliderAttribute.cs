using UnityEngine;

namespace Utils.Core.Attributes
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public float minLimit = 0;
        public float maxLimit = 1;

        public MinMaxSliderAttribute(int min, int max)
        {
            minLimit = min;
            maxLimit = max;
        }
    }
}