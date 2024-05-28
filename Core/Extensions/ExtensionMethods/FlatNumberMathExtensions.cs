using UnityEngine;

namespace Utils.Core.Extensions
{
	public static class FlatNumberMathExtensions
	{
		public static float SmoothStep(float a, float b, float x)
		{
			if (x < a)
				return 0f;
			if (x > b)
				return 1f;

			x = (x - a) / (b - a);

			return x * x * (3f - 2f * x);
		}

		public static float SmoothStepUnclamped(float a, float b, float x)
		{
			x = Mathf.Clamp01((x - a) / (b - a));
			return x * x * (3f - 2f * x);
		}

		public static float GetRawDifference(float a, float b)
		{
			return a > b ? a - b : b - a;
		}

		public static float ClampValueToRange(float low, float high, float value)
		{
			return value < low ? low : value > high ? high : value;
		}

		public static float SubtractRadially(float a, float b)
		{
			bool isNeg = a > b;
			float diff = GetRawDifference(a, b);

			if (diff > 0.5)
				return a - b - (!isNeg ? -1f : 1f);
			else
				return a - b;
		}

		public static bool SqrDistanceIsBiggerNN(float a, float b)
		{
			return a.Pow2() > b.Pow2();
		}

		public static bool SqrDistanceIsBiggerPN(float a, float b)
		{
			return a > b.Pow2();
		}

		public static bool SqrDistanceIsBiggerNP(float a, float b)
		{
			return a.Pow2() > b;
		}

		public static bool SqrDistanceIsBiggerPP(float a, float b)
		{
			return a > b;
		}
		public static float SmoothEaseInOut(float t)
		{
			t = Mathf.Clamp01(t); // Ensures t is in the range [0, 1]
			return Mathf.Sin(t * Mathf.PI - Mathf.PI * 0.5f) * 0.5f + 0.5f;
		}

		/// <summary>
		/// This method adds random fluctuations to an input value
		/// </summary>
		/// <param name="n">Input</param>
		/// <param name="factor">"n" will be modified by a positive or negative random value in domain of this value. Keep within domain [0,1]</param>
		/// <returns></returns>
		public static float Fluctuate(this float n, float factor = 0.1f)
		{
			return n + (n * (UnityEngine.Random.Range(-1, 1) * factor));
		}
	}
}
