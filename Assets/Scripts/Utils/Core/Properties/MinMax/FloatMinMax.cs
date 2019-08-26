using UnityEngine;

namespace Utils.Core
{
    [System.Serializable]
    public struct FloatMinMax
    {
        public float min, max;

        public FloatMinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float GetRandom()
        {
            return Random.Range(min, max);
        }
    }
}