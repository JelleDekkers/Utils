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

		public static void DrawText(string text, Vector3 position, int fontSize = 0)
		{
			DrawText(text, position, Color.black, fontSize);
		}

		public static void DrawText(string text, Vector3 position, Color color, int fontSize = 0)
		{
#if UNITY_EDITOR
			GUIStyle style = new GUIStyle();
			style.normal.textColor = color;
			style.fontSize = fontSize;

			UnityEditor.Handles.BeginGUI();
			UnityEditor.Handles.Label(position, text, style);
			UnityEditor.Handles.EndGUI();
#endif
		}
	}
}