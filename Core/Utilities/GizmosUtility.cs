using UnityEngine;

namespace Utils.Core
{
    public static class GizmosUtility
    { 
        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowHead(pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);
            DrawArrowHead(pos, direction, color, arrowHeadLength, arrowHeadAngle);
        }

        private static void DrawArrowHead(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;

            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Gizmos.DrawRay(pos + direction, up * arrowHeadLength);

            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
            Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
        }
    }
}