using UnityEditor;
using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    public static class HierarchyBranchDrawerHelper
    {
        private static HierarchySettings Settings => HierarchySettings.Instance;
        private static int currentBranch;
        private const float barWidth = 2;

        static float GetStartX(Rect originalRect, int nestLevel)
        {
            return 37 + (originalRect.height - 2) * nestLevel;   
        }

        static Color GetNestColor(int nestLevel)
        {
            if (Settings.childBranchColors == null || Settings.childBranchColors.Count == 0)
                return Color.gray;

            return Settings.childBranchColors[(nestLevel - 1) % Settings.childBranchColors.Count];
        }

        public static void DrawVerticalLine(Rect originalRect, int nestLevel)
        {
            DrawHalfVerticalLine(originalRect, true, nestLevel);
            DrawHalfVerticalLine(originalRect, false, nestLevel);
        }

        public static void DrawHalfVerticalLine(Rect originalRect, bool startsOnTop, int nestLevel)
        {
             DrawHalfVerticalLine(originalRect, startsOnTop, nestLevel, GetNestColor(nestLevel));
        }

        public static void DrawHalfVerticalLine(Rect originalRect, bool startsOnTop, int nestLevel, Color color)
        {
            //Vertical rect, starts from the very left and then proceeds to te right
            EditorGUI.DrawRect(
                new Rect(
                    GetStartX(originalRect, nestLevel),
                    startsOnTop ? originalRect.y : (originalRect.y + originalRect.height / 2f),
                    barWidth,
                    originalRect.height / 2f
                    ),
                color
                );
        }

        public static void DrawHorizontalLine(Rect originalRect, int nestLevel, bool hasChilds)
        {
            //Vertical rect, starts from the very left and then proceeds to te right
            EditorGUI.DrawRect(
                new Rect(
                    GetStartX(originalRect, nestLevel),
                    originalRect.y + originalRect.height / 2f,
                    originalRect.height + (hasChilds ? -5 : 2),
                    //originalRect.height - 5, 
                    barWidth
                    ),
                GetNestColor(nestLevel)
                );
        }
    }
}