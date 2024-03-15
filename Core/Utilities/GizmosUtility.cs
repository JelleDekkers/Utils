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

		public static void DrawText(string text, Vector3 position, Color color, int fontSize = 12)
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

        /// <summary>
        /// Draws text in the scene view that scales with the camera distance
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="scaleFactor"></param>
        /// <param name="baseFontSize"></param>
        public static void DrawTextScaling(string text, Vector3 position, Color color, int baseFontSize = 12, float scaleFactor = 0.5f)
        {
#if UNITY_EDITOR
            UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                if (sceneView.camera != null)
                {
                    float distance = Vector3.Distance(position, sceneView.camera.transform.position);
                    float scaledFontSize = baseFontSize - distance * scaleFactor;

                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = color;
                    style.fontSize = Mathf.Max(1, Mathf.RoundToInt(scaledFontSize));

                    UnityEditor.Handles.BeginGUI();
                    UnityEditor.Handles.Label(position, text, style);
                    UnityEditor.Handles.EndGUI();
                }
            }
#endif
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        public static void DrawBox(Transform translation)
        {
            DrawBox(translation, translation.position, translation.lossyScale);
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawBox(Transform translation, Vector3 center, Vector3 size, Color color)
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = color;
            DrawBox(translation, center, size);
            Gizmos.color = prevColor;
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawBox(Transform translation, Vector3 center, Vector3 size)
        {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, translation.rotation, size);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = prevMatrix;
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        public static void DrawWireBox(Transform translation)
        {
            DrawWireBox(translation, translation.position, translation.lossyScale);
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawWireCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireBox(Transform translation, Vector3 center, Vector3 size, Color color)
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = color;
            DrawWireBox(translation, center, size);
            Gizmos.color = prevColor;
        }

        /// <summary>
        /// Draws a <see cref="Gizmos.DrawWireCube(Vector3, Vector3)"/> that will properly rotate and scale to it's transform 
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireBox(Transform translation, Vector3 center, Vector3 size)
        {
            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, translation.rotation, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = prevMatrix;
        }

        /// <summary>
        /// Draws a partial sphere/arc
        /// </summary>
        /// <param name="position"></param>
        /// <param name="dir"></param>
        /// <param name="anglesRange"></param>
        /// <param name="radius"></param>
        /// <param name="maxSteps"></param>
        public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
        {
            var srcAngles = GetAnglesFromDir(position, dir);
            var initialPos = position;
            var posA = initialPos;
            var stepAngles = anglesRange / maxSteps;
            var angle = srcAngles - anglesRange / 2;
            for (int i = 0; i <= maxSteps; i++)
            {
                var rad = Mathf.Deg2Rad * angle;
                var posB = initialPos;
                posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                Gizmos.DrawLine(posA, posB);

                angle += stepAngles;
                posA = posB;
            }
            Gizmos.DrawLine(posA, initialPos);
        }

        static float GetAnglesFromDir(Vector3 position, Vector3 dir)
        {
            var forwardLimitPos = position + dir;
            var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

            return srcAngles;
        }
    }
}