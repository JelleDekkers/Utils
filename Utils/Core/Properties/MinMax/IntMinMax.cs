using UnityEngine;

namespace Utils.Core
{
    [System.Serializable]
    public struct IntMinMax
    {
        public int min, max;

        public IntMinMax(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int GetRandom()
        {
            return Random.Range(min, max);
        }
    }
}