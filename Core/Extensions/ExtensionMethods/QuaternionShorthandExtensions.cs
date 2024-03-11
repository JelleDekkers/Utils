using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
	public static class QuaternionShortHandExtensions
		{
			public static Quaternion Inverse(this Quaternion q)
			{
				return Quaternion.Inverse(q);
			}

			public static Quaternion Normalize(this Quaternion q)
			{
				float f = 1f / Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
				return new Quaternion(q.x * f, q.y * f, q.z * f, q.w * f);
			}
		}
}
