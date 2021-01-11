using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    /// <summary>
    /// Draws a distinctive divider in the scene hierachy with changeable color. Used for grouping together objects in the hierarchy.
    /// </summary>
    public class HierarchySeperator : MonoBehaviour
    {
#if UNITY_EDITOR
        public Color titleBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
        public Color childBackgroundColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
#endif
    }
}