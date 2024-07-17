using UnityEngine;

namespace Utils.Core.Extensions
{
	public static class VectorShortHandExtensions
	{
		public static float GetHighest(this Vector3 v3)
		{
			return Mathf.Max(v3.x, v3.y, v3.z);
		}

		public static float GetLowest(this Vector3 v3)
		{
			return Mathf.Min(v3.x, v3.y, v3.z);
		}

		public static float Max(this Vector2 v)
		{
			return Mathf.Max(v.x, v.y);
		}

		public static float Min(this Vector2 v)
		{
			return Mathf.Min(v.x, v.y);
		}

		public static float Max(this Vector3 v)
		{
			return Mathf.Max(v.x, v.y, v.z);
		}

		public static float Min(this Vector3 v)
		{
			return Mathf.Min(v.x, v.y, v.z);
		}

		public static float Max(this Vector4 v)
		{
			return Mathf.Max(v.x, v.y, v.z, v.w);
		}

		public static float Min(this Vector4 v)
		{
			return Mathf.Min(v.x, v.y, v.z, v.w);
		}

		/// <summary>
		/// Removes the passed component from a Vector3.
		/// </summary>
		/// 
		public static Vector2 Flatten(this Vector2 v2, WorldPlanes plane)
		{
			//float[] fs = new float[2] { v2.x, v2.y };
			//fs[(int)plane] = 0;
			//return new Vector2(fs[0], fs[1]);
			if (plane == WorldPlanes.X || plane == WorldPlanes.x)
				v2.x = 0f;
			else if (plane == WorldPlanes.Y || plane == WorldPlanes.y)
				v2.y = 0f;

			return v2;
		}

		/// <summary>
		/// Removes the passed component from a Vector3.
		/// </summary>
		/// 
		public static Vector3 Flatten(this Vector3 v3, WorldPlanes plane)
		{
			//float[] fs = new float[3] { v3.x, v3.y, v3.z };
			//fs[(int)plane] = 0;
			//return new Vector3(fs[0], fs[1], fs[2]);
			if (plane == WorldPlanes.X || plane == WorldPlanes.x)
				v3.x = 0f;
			else if (plane == WorldPlanes.Y || plane == WorldPlanes.y)
				v3.y = 0f;
			else if (plane == WorldPlanes.Z || plane == WorldPlanes.z)
				v3.z = 0f;

			return v3;
		}

		public static Vector3 Flatten(this Vector3 v3, string input)
		{
			bool x = input.Contains("x");
			bool y = input.Contains("y");
			bool z = input.Contains("z");

			float[] fs = new float[3] { v3.x, v3.y, v3.z };
			fs[0] = (!x) ? fs[0] : 0;
			fs[1] = (!y) ? fs[1] : 0;
			fs[2] = (!z) ? fs[2] : 0;
			return new Vector3(fs[0], fs[1], fs[2]);
		}

		/// <summary>
		/// Removes the passed component from a Vector3.
		/// </summary>
		/// 
		public static Vector4 Flatten(this Vector4 v4, WorldPlanes plane)
		{
			//float[] fs = new float[4] { v4.x, v4.y, v4.z, v4.w };
			//fs[(int)plane] = 0;
			//return new Vector4(fs[0], fs[1], fs[2], fs[3]);

			if (plane == WorldPlanes.X || plane == WorldPlanes.x)
				v4.x = 0f;
			else if (plane == WorldPlanes.Y || plane == WorldPlanes.y)
				v4.y = 0f;
			else if (plane == WorldPlanes.Z || plane == WorldPlanes.z)
				v4.z = 0f;

			return v4;
		}

		public static Vector2 Swizzle(this Vector2 v2, string input)
		{
			int count = input.Length;

			if (count == 0)
				return v2;
			if (count == 1)
			{
				input += input[0];
			}
			count = 2;

			float[] returnV = new float[2] { v2.x, v2.y };

			for (int i = 0; i < count; i++)
			{
				switch (input[i].ToString())
				{
					case "x":
						returnV[i] = v2.x;
						break;
					case "y":
						returnV[i] = v2.y;
						break;
					case "1":
						returnV[i] = 1;
						break;
					case "0":
						returnV[i] = 0;
						break;
				}
			}
			return new Vector2(returnV[0], returnV[1]);
		}

		public static Vector3 Swizzle(this Vector3 v3, string input)
		{
			int count = input.Length;

			if (count == 0)
				return v3;
			if (count == 1)
			{
				input += input[0];
				input += input[0];
			}
			if (count == 2)
			{
				input += input[1];
			}
			count = 3;

			float[] returnV = new float[3] { v3.x, v3.y, v3.z };

			for (int i = 0; i < count; i++)
			{
				switch (input[i].ToString())
				{
					case "x":
						returnV[i] = v3.x;
						break;
					case "y":
						returnV[i] = v3.y;
						break;
					case "z":
						returnV[i] = v3.z;
						break;
					case "1":
						returnV[i] = 1;
						break;
					case "0":
						returnV[i] = 0;
						break;
				}
			}
			return new Vector3(returnV[0], returnV[1], returnV[2]);
		}

		public static Vector4 Swizzle(this Vector4 v4, string input)
		{
			int count = input.Length;

			if (count == 0)
				return v4;
			if (count == 1)
			{
				input += input[0];
				input += input[0];
				input += input[0];
			}
			if (count == 2)
			{
				input += input[1];
				input += input[1];
			}
			if (count == 3)
			{
				input += input[2];
			}
			count = 4;

			float[] returnV = new float[4] { v4.x, v4.y, v4.z, v4.w };

			for (int i = 0; i < count; i++)
			{
				switch (input[i].ToString())
				{
					case "x":
						returnV[i] = v4.x;
						break;
					case "y":
						returnV[i] = v4.y;
						break;
					case "z":
						returnV[i] = v4.z;
						break;
					case "w":
						returnV[i] = v4.w;
						break;
					case "1":
						returnV[i] = 1;
						break;
					case "0":
						returnV[i] = 0;
						break;
				}
			}
			return new Vector4(returnV[0], returnV[1], returnV[2], returnV[3]);
		}

		public static Vector3 Mul(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3 Mul(this Vector3 a, float x, float y, float z)
		{
			return new Vector3(a.x * x, a.y * y, a.z * z);
		}

		public static Vector3 Div(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector3 Div(this Vector3 a, float x, float y, float z)
		{
			return new Vector3(a.x / x, a.y / y, a.z / z);
		}

		public static Vector3 Replace(this Vector3 v3, WorldPlanes plane, float value)
		{
			float[] fs = new float[3] { v3.x, v3.y, v3.z };
			fs[(int)plane] = value;
			return new Vector3(fs[0], fs[1], fs[2]);
		}

		public static float InverseLerp(this Vector2 value, Vector2 a, Vector2 b)
		{
			Vector2 AB = b - a;
			Vector2 AV = value - a;
			return Vector2.Dot(AV, AB) / Vector2.Dot(AB, AB);
		}

		public static float InverseLerp(this Vector3 value, Vector3 a, Vector3 b)
		{
			Vector3 AB = b - a;
			Vector3 AV = value - a;
			return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
		}
		public static Vector3 Limit(this Vector3 v3, float magnitude)
		{
			if (v3.sqrMagnitude > magnitude.Pow2())
				return v3.normalized * magnitude;
			else
				return v3;
		}

		public static (Vector3 worldPosition, Quaternion worldRotation) ToTuple(this Transform target)
		{
			return (target.position, target.rotation);
		}

		public static float Area(this Vector2 v2)
		{
			return (v2.x * v2.y).Abs();
		}

		public static float Volume(this Vector3 v3)
		{
			return (v3.x * v3.y * v3.z).Abs();
		}

		public static float Volume(this Vector4 v4)
		{
			return (v4.x * v4.y * v4.z * v4.w).Abs();
		}
	}
}
