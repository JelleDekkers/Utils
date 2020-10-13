
using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Returns normalized value (0-1) between a minimum and maximum
        /// Example: float lerp = velocity.GetNormalizedValue(minVelocity, maxVelocity);
        /// newFov = Mathf.Lerp(normalFov, maxFov, lerp);
        /// </summary>
        public static float GetNormalizedValue(this float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        /// <summary>
        /// A map function to move a given number from scope A to scope B.
        /// </summary>
        public static float Map(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            float result = (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
            if (result > toMax) return toMax;
            if (result < toMin) return toMin;
            return result;
        }

        /// <summary>
        /// Returns a proper modulo which works with negative numbers
        /// </summary>
        /// <returns></returns>
        public static float Modulo(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }
    }
}
