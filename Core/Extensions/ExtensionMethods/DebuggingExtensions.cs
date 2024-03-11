
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
	public static class Debugging
	{
		public static T Log<T>(this T var, string prefix = "", string suffix = "")
		{
			Debug.Log($"{prefix} {var} {suffix}");
			return var;
		}

		public static Vector4 Log(this Vector4 v, string prefix = "", string suffix = "")
		{
			Debug.Log($"{prefix} ({v.x},{v.y},{v.z},{v.w}) {suffix}");
			return v;
		}

		public static Vector3 Log(this Vector3 v, string prefix = "", string suffix = "")
		{
			Debug.Log($"{prefix} ({v.x},{v.y},{v.z}) {suffix}");
			return v;
		}

		public static Vector2 Log(this Vector2 v, string prefix = "", string suffix = "")
		{
			Debug.Log($"{prefix} ({v.x},{v.y}) {suffix}");
			return v;
		}




		public static GameObject GenerateDebugObject(PrimitiveType type, Vector3 scale)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			go.GetComponent<Collider>().enabled = false;
			go.transform.localScale = scale;
			return go;
		}

		public static GameObject GenerateDebugObject(PrimitiveType type, float scale)
		{
			GameObject go = GameObject.CreatePrimitive(type);
			go.GetComponent<Collider>().enabled = false;
			go.transform.localScale = Vector3.one * scale;
			return go;
		}
	}
}
