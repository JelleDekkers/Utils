using UnityEngine;

namespace Utils.Core.Attributes
{
    public class CurveAttribute : PropertyAttribute
    {
        public float PosX, PosY;
        public float RangeX, RangeY;
        public int x;

        public CurveAttribute(float PosX, float PosY, float RangeX, float RangeY)
        {
            this.PosX = PosX;
            this.PosY = PosY;
            this.RangeX = RangeX;
            this.RangeY = RangeY;
        }
    }
}