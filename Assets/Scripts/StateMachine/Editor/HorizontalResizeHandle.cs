using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class HorizontalResizeHandle
    {
        public Rect HandleRect => handleRect;
        private Rect handleRect;

        private bool isDragging;
        private float minLimit, maxLimit;

        public HorizontalResizeHandle(float minLimit, float maxLimit)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }

        public void Draw(float height = 5)
        {
            handleRect = GUILayoutUtility.GetLastRect();
            handleRect.y -= HandleRect.height / 2;
            handleRect.height = height;

            GUI.Box(HandleRect, "", GUIStyle.none);
            if (HandleRect.Contains(Event.current.mousePosition) || isDragging)
            {
                EditorGUIUtility.AddCursorRect(HandleRect, MouseCursor.ResizeVertical);
            }
        }

        public void ProcessEvents(Event e, ref Rect rect)
        {
            if (e.button == 0)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (handleRect.Contains(e.mousePosition))
                    {
                        isDragging = true;
                        e.Use();
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    isDragging = false;
                }
                else if (e.type == EventType.MouseDrag)
                {
                    if (isDragging)
                    {
                        ResizeRect(e.delta, ref rect);
                        e.Use();
                    }
                }
            }
        }

        public void ResizeRect(Vector2 delta, ref Rect rect)
        {
            float newHeight = rect.height + delta.y;
            if (newHeight >= minLimit && newHeight <= maxLimit)
            {
                rect = new Rect(rect.x, rect.y, rect.width, newHeight);
                GUI.changed = true;
            }
        }
    }
}