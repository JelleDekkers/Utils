
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
	public static class FlatNumberShortHandExtensions
		{
			public static float Sqrt(this float f)
			{
				return Mathf.Sqrt(f);
			}

			public static float OneMinus(this float f)
			{
				return 1f - f;
			}
			/// <summary>
			/// Moves the value from the from domain to the to domain.
			/// </summary>
			public static float Remap(this float value, float from1, float to1, float from2, float to2)
			{
				return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
			}

			/// <summary>
			/// Returns the decimal value.
			/// </summary>
			public static float Fraq(this float val)
			{
				return val - Mathf.Floor(val);
			}

			public static float Floor(this float val)
			{
				return Mathf.Floor(val);
			}

			public static float Ceil(this float val)
			{
				return Mathf.Ceil(val);
			}

			public static float Round(this float val)
			{
				return Mathf.Round(val);
			}

			public static float Abs(this float val)
			{
				return Mathf.Abs(val);
			}

			public static float Sign(this float val)
			{
				return Mathf.Sign(val);
			}

			public static float Clamp(this float val, float low, float high)
			{
				return Mathf.Clamp(val, low, high);
			}

			public static float Clamp(this float val, Vector2 range)
			{
				return val.Clamp(range.x, range.y);
			}

			public static float Clamp01(this float val)
			{
				return val.Clamp(0f, 1f);
			}

			public static float Pow(this float f, float p)
			{
				return Mathf.Pow(f, p);
			}

			public static float Pow2(this float f)
			{
				return f * f;
			}

			public static bool Flip(this ref bool b)
			{
				b = !b;
				return b;
			}
		}
}
