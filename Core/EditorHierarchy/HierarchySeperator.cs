using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    /// <summary>
    /// Draws a distinctive divider in the scene hierachy with changeable color. Used for grouping together objects in the hierarchy.
    /// </summary>
    public class HierarchySeperator : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color titleBackgroundColor;
        public Color childBackgroundColor;
#endif
    }
}