using UnityEngine;

namespace ExtensionMethods
{
	public static class QuaternionMathExtensions
		{
			public static Quaternion ToLocal(this Quaternion rot, Transform reference)
			{
				return Quaternion.Inverse(reference.rotation) * rot;
			}

			public static Quaternion ToGlobal(this Quaternion rot, Transform reference)
			{
				return reference.rotation * rot;
			}

			public static (Vector3 forward, Vector3 up, Vector3 right) GetDirectionVectors(this Quaternion rotation)
			{
				Vector3 forward = rotation * Vector3.forward;
				Vector3 up = rotation * Vector3.up;
				Vector3 right = rotation * Vector3.right;

				return (forward, up, right);
			}
		}
}
