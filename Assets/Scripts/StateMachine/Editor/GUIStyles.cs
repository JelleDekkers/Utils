using UnityEngine;

namespace Utils.Core.Flow
{
    public static class GUIStyles
    {
        public const float KNOB_SIZE = 10;

        public static readonly Color KNOB_COLOR_LINKED = new Color(0f, 0.8f, 0.3f, 1f);
        public static readonly Color KNOB_COLOR_EMPTY = Color.red;
        public static readonly Color HIGHLIGHT_OUTLINE_COLOR = Color.yellow;
        public static readonly Color LINK_COLOR = Color.red;
        public static readonly Color LINK_COLOR_SELECTED = Color.yellow;
        public static readonly Color CURRENT_ACTIVE_STATE_OUTLINE = Color.green;

        private static GUIStyle canvasWindowStyle;
        public static GUIStyle CanvasWindowStyle
        {
            get
            {
                if(canvasWindowStyle == null)
                {
                    canvasWindowStyle = new GUIStyle("Box");
                    canvasWindowStyle.padding = new RectOffset();
                    canvasWindowStyle.margin = new RectOffset(0, 0, -(int)UnityEditor.EditorStyles.toolbar.fixedHeight, 0);
                }

                return canvasWindowStyle;
            }
        }

        private static GUIStyle stateHeaderStyle;
        public static GUIStyle StateHeaderStyle
        {
            get
            {
                if (stateHeaderStyle == null)
                {
                    stateHeaderStyle = new GUIStyle("Box")
                    {
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return stateHeaderStyle;
            }
        }

        private static GUIStyle stateToolbarButtonsStyle;
        public static GUIStyle StateToolbarButtonsStyle
        {
            get
            {
                if(stateToolbarButtonsStyle == null)
                {
                    stateToolbarButtonsStyle = new GUIStyle("ToolbarButton")
                    {
                        padding = new RectOffset()
                    };
                }

                return stateToolbarButtonsStyle;
            }
        }

        public static GUIStyle RuleGroupStyle
        {
            get
            {
                if (ruleGroupStyle == null)
                {
                    ruleGroupStyle = new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleRight,
                        padding = new RectOffset(10, 10, 3, 3),
                        wordWrap = true
                    };
                }
                return ruleGroupStyle;
            }
        }
        private static GUIStyle ruleGroupStyle;

        public static GUIStyle RuleGroupKnobStyle
        {
            get
            {
                if (ruleGroupKnobStyle == null)
                {
                    ruleGroupKnobStyle = new GUIStyle();
                    ruleGroupKnobStyle.alignment = TextAnchor.MiddleCenter;
                    ruleGroupKnobStyle.normal.background = Resources.Load<Texture2D>("Knob");
                }
                return ruleGroupKnobStyle;
            }
        }
        private static GUIStyle ruleGroupKnobStyle;

        private static GUIStyle ruleGroupOutlineStyle;
        public static GUIStyle RuleGroupOutlineStyle
        {
            get
            {
                if(ruleGroupOutlineStyle == null)
                {
                    ruleGroupOutlineStyle = new GUIStyle();
                    ruleGroupOutlineStyle.border = new RectOffset(4, 4, 4, 4);
                    ruleGroupOutlineStyle.padding = new RectOffset(4, 4, 4, 4);
                    ruleGroupOutlineStyle.normal.background = OutlineTexture;
                }

                return ruleGroupOutlineStyle;
            }
        }

        private static Texture2D outlineTexture;
        private static Texture2D OutlineTexture
        {
            get
            {
                if (outlineTexture == null)
                {
                    outlineTexture = DrawHelper.CreateBoxTexture(Color.clear, Color.white, Color.white, Color.white, Color.white);
                }

                return outlineTexture;
            }
        }

        private static GUIStyle inspectorStyle;
        public static GUIStyle InspectorStyle
        {
            get
            {
                if (inspectorStyle == null)
                {
                    inspectorStyle = new GUIStyle();
                    inspectorStyle.margin = new RectOffset(15, 0, 0, 0);
                }

                return inspectorStyle;
            }
        }

        private static GUIStyle bezierLineHandleStyle;
        public static GUIStyle BezierLineHandleStyle
        {
            get
            {
                if (bezierLineHandleStyle == null)
                {
                    bezierLineHandleStyle = new GUIStyle();
                    bezierLineHandleStyle.normal.background = Resources.Load<Texture2D>("Knob");
                }

                return bezierLineHandleStyle;
            }
        }
    }
}