using UnityEngine;

namespace ExtensionMethods
{
	public static class VectorMathExtensions
	{
		public static bool Vector3IsInRange(Vector3 a, Vector3 b, float length)
		{
			return VectorMathExtensions.SqrDistance(a, b) <= length * length;
		}

		public static bool Vector3IsInRangeWeighted(Vector3 a, Vector3 b, float length, float xMultiplier = 1, float yMultiplier = 1, float zMultiplier = 1)
		{
			float dist = VectorMathExtensions.SqrDistance(a, b);
			return VectorMathExtensions.SqrDistanceWithMultipliers(a, b, xMultiplier, yMultiplier, zMultiplier) <= length * length;
		}

		public static float CompareVolumes(Vector3 vec1, Vector3 vec2)
		{
			float volume1 = vec1.Volume();
			float volume2 = vec2.Volume();

			float diff = Mathf.Abs(volume1 - volume2);

			float maxVolume = Mathf.Max(volume1, volume2);

			return Mathf.Clamp01(diff / maxVolume);
		}


		public static float SqrDistance(this Vector3 a, Vector3 b)
		{
			return (b - a).sqrMagnitude;
		}


		public static float Random(this Vector2 v2)
		{
			return UnityEngine.Random.Range(v2.x, v2.y);
		}

		public static float RandomInclusive(this Vector2 v2)
		{
			return UnityEngine.Random.Range(v2.x, maxInclusive: v2.y);
		}

		public static int Random(this Vector2Int v2)
		{
			return UnityEngine.Random.Range(v2.x, v2.y);
		}

		public static float RandomInclusive(this Vector2Int v2)
		{
			return UnityEngine.Random.Range(v2.x, maxInclusive: v2.y);
		}

		public static float Lerp(this Vector2 v2, float t)
		{
			return Mathf.Lerp(v2.x, v2.y, t);
		}

		public static float InverseLerp(this Vector2 v2, float t)
		{
			return Mathf.InverseLerp(v2.x, v2.y, t);
		}

		public static float AngleBetweenVectors(Vector3 v1, Vector3 v2, Vector3 axis)
		{
			axis.Normalize();

			Vector3 cross = Vector3.Cross(v1, v2);

			float dot = Vector3.Dot(v1, v2);

			float angle = Mathf.Atan2(cross.magnitude, dot);

			angle = angle * Mathf.Rad2Deg;

			float sign = Mathf.Sign(Vector3.Dot(cross, axis));
			float signedAngle = angle * sign;

			return (signedAngle + 360) % 360;
		}

		public static Vector3 RotateAround(this Vector3 position, Vector3 centerPoint, Vector3 axis, float rotationAngle)
		{
			Quaternion rotation = Quaternion.AngleAxis(rotationAngle, axis);
			Vector3 offset = position - centerPoint;
			return centerPoint + rotation * offset;
		}

		public static Vector3 RotateAroundLocal(this Vector3 position, Vector3 centerPoint, Vector3 axis, float rotationAngle)
		{
			Quaternion rotation = Quaternion.AngleAxis(rotationAngle, axis);
			Vector3 localAxis = position - centerPoint;
			localAxis = Quaternion.Inverse(rotation) * localAxis;
			Vector3 rotatedOffset = rotation * localAxis;
			return centerPoint + rotatedOffset;
		}

		public static float SqrDistanceWithMultipliers(Vector3 v1, Vector3 v2, float xMultiplier = 1, float yMultiplier = 1, float zMultiplier = 1)
		{
			Vector3 diff = new Vector3((v1.x - v2.x) * xMultiplier, (v1.y - v2.y) * yMultiplier, (v1.z - v2.z) * zMultiplier);
			return diff.sqrMagnitude;
		}
	}
}
