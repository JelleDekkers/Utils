using UnityEngine;

namespace Utils.Core
{
    public class MathUtility
    {
        public static float FullAngle(Vector3 a, Vector3 b, Vector3 reference)
        {
            a = a.normalized;
            b = b.normalized;
            float angle = Vector3.Angle(a, b);
            float dot = Vector3.Dot(b, Vector3.Cross(a, reference));
            if (dot > 0)
            {
                return 360f - angle;
            }
            else
            {
                return angle;
            }
        }

        public static float SignedAngle(Vector3 a, Vector3 b)
        {
            return SignedAngle(a, b, Vector3.up);
        }

        public static float SignedAngle(Vector3 a, Vector3 b, Vector3 reference)
        {
            float angle = FullAngle(a, b, reference);
            if (angle > 180f)
            {
                return angle - 360f;
            }
            else
            {
                return angle;
            }
        }

		public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
		{
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}
	}
}