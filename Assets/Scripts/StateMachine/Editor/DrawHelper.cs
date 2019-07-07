using UnityEngine;

namespace StateMachine
{
    public static class DrawHelper
    {
        public static void DrawBoxOutline(Rect rect, Color color)
        {
            GUIStyle borderStyle = GUIStyles.RuleGroupOutlineStyle;

            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.Box(rect, "", borderStyle);

            GUI.color = previousColor;
        }

        public static Texture2D CreateBoxTexture(Color fillColor, Color borderColor)
        {
            return CreateBoxTexture(fillColor, borderColor, borderColor, borderColor, borderColor);
        }

        public static Texture2D CreateBoxTexture(Color fillColor, Color topBorderColor, Color bottomBorderColor, Color leftBorderColor, Color rightBorderColor)
        {
            Color c = fillColor;
            Color t = topBorderColor;
            Color b = bottomBorderColor;
            Color l = leftBorderColor;
            Color r = rightBorderColor;

            Texture2D boxTexture = new Texture2D(8, 8, TextureFormat.ARGB32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            boxTexture.SetPixels32
            (
                new Color32[]
                {
                    l, l, b, b, b, b, r, r,
                    l, l, b, b, b, b, r, r,
                    l, l, c, c, c, c, r, r,
                    l, l, c, c, c, c, r, r,
                    l, l, c, c, c, c, r, r,
                    l, l, c, c, c, c, r, r,
                    l, l, t, t, t, t, r, r,
                    l, l, t, t, t, t, r, r
                }
            );
            boxTexture.wrapMode = TextureWrapMode.Clamp;
            boxTexture.Apply();

            return boxTexture;
        }

        public static void DrawRuleHandleKnob(Rect ruleRect, System.Action onKnobPressed, Color color, float handleSize = GUIStyles.KNOB_SIZE)
        {
            Rect rect = new Rect(ruleRect.x - handleSize / 2, ruleRect.position.y - handleSize / 2, handleSize, handleSize);

            Color prevColor = GUI.color;
            GUI.color = color;
            if (GUI.Button(rect, "", GUIStyles.RuleGroupKnobStyle))
            {
                onKnobPressed?.Invoke();
            }
            GUI.color = prevColor;
        }
    }
}