using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class QuadraticSpline
	{
		public static (Vector3 Position, Quaternion Rotation) SampleSpline(float t, (Vector3 position, Quaternion rotation) pointA, (Vector3 position, Quaternion rotation) pointB, (Vector3 position, Quaternion rotation) pointC)
		{
			Vector3 position = CalculateQuadraticBezierPoint(t, pointA.position, pointB.position, pointC.position);
			Quaternion rotation = Quaternion.Slerp(Quaternion.Slerp(pointA.rotation, pointB.rotation, t), Quaternion.Slerp(pointB.rotation, pointC.rotation, t), t);
			return (position, rotation);
		}

		public static (Vector3 Position, Quaternion Rotation) SampleSpline(float t, Transform pointA, Transform pointB, Transform pointC)
		{
			return SampleSpline(t, (pointA.position, pointA.rotation), (pointB.position, pointB.rotation), (pointC.position, pointC.rotation));
		}

		public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
		{
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;

			Vector3 p = uu * p0; // first term
			p += 2 * u * t * p1; // second term
			p += tt * p2; // third term

			return p;
		}

		public static float MoveTowardsTargetTWithCurrentPosition((Vector3 position, Quaternion rotation) pointA, (Vector3 position, Quaternion rotation) pointB, (Vector3 position, Quaternion rotation) pointC, Vector3 currentPosition, float targetT, float maxDistanceDelta)
		{
			float currentT = FindClosestTForPosition(pointA, pointB, pointC, currentPosition);
			return MoveTowardsTargetT(pointA, pointB, pointC, currentT, targetT, maxDistanceDelta);
		}

		public static float FindClosestTForPosition((Vector3 position, Quaternion rotation) pointA, (Vector3 position, Quaternion rotation) pointB, (Vector3 position, Quaternion rotation) pointC, Vector3 currentPosition)
		{
			float closestT = 0f;
			float minDistance = float.MaxValue;
			float step = 0.01f; // Precision for searching

			for (float t = 0; t <= 1; t += step)
			{
				var (position, _) = SampleSpline(t, pointA, pointB, pointC);
				float distance = Vector3.Distance(position, currentPosition);
				if (distance < minDistance)
				{
					minDistance = distance;
					closestT = t;
				}
			}

			return closestT;
		}

		public static float MoveTowardsTargetT((Vector3 position, Quaternion rotation) pointA, (Vector3 position, Quaternion rotation) pointB, (Vector3 position, Quaternion rotation) pointC, float currentT, float targetT, float maxDistanceDelta)
		{
			var (currentPosition, _) = SampleSpline(currentT, pointA, pointB, pointC);
			var (targetPosition, _) = SampleSpline(targetT, pointA, pointB, pointC);

			float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
			if (distanceToTarget < maxDistanceDelta)
			{
				return targetT;
			}

			float direction = targetT > currentT ? 1.0f : -1.0f;
			float bestT = currentT;
			float bestDistance = distanceToTarget;
			float step = 0.01f;

			while (step >= 0.0001f)
			{
				float newT = currentT + direction * step;
				newT = Mathf.Clamp(newT, 0f, 1f);

				var (newPosition, _) = SampleSpline(newT, pointA, pointB, pointC);
				float newDistance = Vector3.Distance(newPosition, targetPosition);

				if (newDistance <= maxDistanceDelta)
				{
					return newT;
				}

				if (newDistance < bestDistance)
				{
					bestT = newT;
					bestDistance = newDistance;
				}
				else
				{
					step *= 0.5f;
				}

				currentT = bestT;
			}

			return bestT;
		}
	}
}
