using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
	public enum WorldPlanes
	{
		X = 0,
		x = 0,
		Y = 1,
		y = 1,
		Z = 2,
		z = 2
	}

	[System.Flags]
	public enum WorldPlanesFlag
	{
		None = 0,
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}

	public static class GeneralExtensions
	{

		public static bool Contains(this LayerMask mask, int layer)
		{
			return ((mask & (1 << layer)) != 0);
		}

		public static Bounds GetTotalBounds(this Transform transform)
		{
			MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
			Collider[] colliders = transform.GetComponentsInChildren<Collider>();

			Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
			bool firstBounds = true;

			foreach (MeshRenderer renderer in meshRenderers)
			{
				if (renderer.gameObject.GetComponent<ParticleSystem>() != null)
				{
					// skip mesh renderer
					continue;
				}

				if (firstBounds)
				{
					totalBounds = renderer.bounds;
					firstBounds = false;
				}
				else
				{
					totalBounds.Encapsulate(renderer.bounds);
				}
			}

			foreach (Collider collider in colliders)
			{
				if (collider.gameObject.GetComponent<ParticleSystem>() != null)
					continue;
				if (firstBounds)
				{
					totalBounds = collider.bounds;
					firstBounds = false;
				}
				else
				{
					totalBounds.Encapsulate(collider.bounds);
				}
			}

			return totalBounds;
		}

		public static Bounds GetRendererBounds(this Transform transform)
		{
			MeshRenderer[] meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();

			Bounds totalBounds = new Bounds(Vector3.zero, Vector3.zero);
			bool firstBounds = true;

			foreach (MeshRenderer renderer in meshRenderers)
			{
				if (renderer.gameObject.GetComponent<ParticleSystem>() != null)
				{
					// skip mesh renderer
					continue;
				}

				if (firstBounds)
				{
					totalBounds = renderer.bounds;
					firstBounds = false;
				}
				else
				{
					totalBounds.Encapsulate(renderer.bounds);
				}
			}

			return totalBounds;
		}
		public static Vector3 ClampToBounds(this Vector3 vector, Bounds bounds)
		{
			return new Vector3(
				Mathf.Clamp(vector.x, bounds.min.x, bounds.max.x),
				Mathf.Clamp(vector.y, bounds.min.y, bounds.max.y),
				Mathf.Clamp(vector.z, bounds.min.z, bounds.max.z)
			);
		}

		public static Vector3 ClampToBounds(this Vector3 vector, Transform parent)
		{
			Bounds bounds = parent.GetTotalBounds();

			return ClampToBounds(vector, bounds);
		}
		/// <summary>
		/// Transforms position from local space to world space. Needs reference transform
		/// </summary>
		public static Vector3 ToGlobal(this Vector3 pos, Transform reference)
		{
			return reference.TransformPoint(pos);
		}

		/// <summary>
		/// Transforms position from world space to local space. Needs reference transform
		/// </summary>
		public static Vector3 ToLocal(this Vector3 pos, Transform reference)
		{
			return reference.InverseTransformPoint(pos);
		}

		/// <summary>
		/// Transforms position from local space to world space. Needs reference transform
		/// </summary>
		public static Vector3 ToGlobalWithPivotAdjustment(this Vector3 pos, Transform reference)
		{
			return reference.TransformPoint(pos) - reference.GetPivotOffset();
		}

		public static Vector3 GetPivotOffset(this Transform t)
		{
			Vector3 pivotPos = t.position;
			Vector3 centerPos = t.GetTotalBounds().center;
			return (centerPos - pivotPos)/* * 0.5f*/;
		}

		public static Vector3 GetTotalPivotOffset(this Transform t)
		{
			Vector3 pivotPos = t.position;
			Vector3 centerPos = t.GetRendererBounds().center;
			return (centerPos - pivotPos)/* * 0.5f*/;
		}

		public static Bounds Scale(this Bounds bounds, float scale)
		{
			return new Bounds(bounds.center, bounds.size * scale);
		}

		public static List<Vector3> GetWorldSpaceCorners(this BoxCollider boxCollider, float scale = 1.0f)
		{
			// World space center position of the BoxCollider
			Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

			Vector3 halfSize = boxCollider.size / 2f;

			List<Vector3> corners = new List<Vector3>();

			corners.Add(worldCenter);
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, halfSize.y, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, halfSize.y, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, halfSize.y, -halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z) * scale));

			return corners;
		}

		public static List<Vector3> GetWorldSpaceCorners(this Bounds bounds, float scale = 1.0f)
		{
			Vector3 halfSize = bounds.extents * scale; // bounds.extents is already half size

			List<Vector3> corners = new List<Vector3>
	{
		bounds.center + new Vector3(halfSize.x, halfSize.y, halfSize.z),
		bounds.center + new Vector3(-halfSize.x, halfSize.y, halfSize.z),
		bounds.center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
		bounds.center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
		bounds.center + new Vector3(halfSize.x, halfSize.y, -halfSize.z),
		bounds.center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
		bounds.center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
		bounds.center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z)
	};

			return corners;
		}

		public static List<Vector3> GetWorldSpaceDiamond(this BoxCollider boxCollider, float scale = 1.0f)
		{
			Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

			Vector3 halfSize = boxCollider.size / 2f;

			List<Vector3> corners = new List<Vector3>();

			corners.Add(worldCenter);
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, halfSize.y, 0.0f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, -halfSize.y, 0.0f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, halfSize.z * 0.5f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, -halfSize.z * 0.5f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, -halfSize.z) * scale));

			return corners;
		}

		public static List<Vector3> GetWorldSpaceDiamondLowRes(this BoxCollider boxCollider, float scale = 1.0f)
		{
			Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

			Vector3 halfSize = boxCollider.size / 2f;

			List<Vector3> corners = new List<Vector3>();

			corners.Add(worldCenter);
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, halfSize.y, 0.0f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, -halfSize.y, 0.0f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0.0f, 0.0f, -halfSize.z) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(halfSize.x, 0.0f, 0.0f) * scale));
			corners.Add(boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(-halfSize.x, 0.0f, 0.0f) * scale));

			return corners;
		}

		public static float Volume(this BoxCollider boxCollider)
		{
			return boxCollider.size.Mul(boxCollider.transform.lossyScale).Volume();
		}


		public static Rect GetCenteredClampedRect(Vector2 position, Vector2 size)
		{
			float screenWidth = Screen.currentResolution.width;
			float screenHeight = Screen.currentResolution.height;

			position.x -= size.x * 0.5f;
			position.y -= 20f;

			position.x = position.x.Clamp(0, screenWidth - size.x);
			position.y = position.y.Clamp(0, screenHeight - size.y);

			return new Rect(position, size);
		}

		public static Rect GetCenteredClampedRectAtMousePosition(Vector2 size)
		{
			Vector2 position = Event.current.mousePosition;
			return GetCenteredClampedRect(position, size);
		}

		public static bool IsInsideBounds(this Vector3 point, Vector3 min, Vector3 max)
		{
			return point.x >= min.x && point.x <= max.x &&
				   point.y >= min.y && point.y <= max.y &&
				   point.z >= min.z && point.z <= max.z;
		}

		public static float IsPositionInColliders(List<Collider> collidersToCheck, Vector3 comparePosition, float stepDistance)
		{
			float fade = 0.0f;
			float minDist = float.MaxValue;

			foreach (var collider in collidersToCheck)
			{
				Vector3 castedPosition = collider.ClosestPoint(comparePosition);
				float dist = VectorMathExtensions.SqrDistance(castedPosition, comparePosition);

				if (dist < minDist)
					minDist = dist;
			}

			if (stepDistance != float.MaxValue)
				fade = Mathf.InverseLerp(0, stepDistance.Pow2(), minDist);
			return fade;
		}

		public static bool IsPositionInColliders(List<Collider> collidersToCheck, Vector3 comparePosition)
		{
			foreach (var collider in collidersToCheck)
			{
				Vector3 castedPosition = collider.ClosestPoint(comparePosition);
				if (castedPosition == comparePosition)
					return true;
			}

			return false;
		}
	}
}