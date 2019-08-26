using UnityEngine;

namespace Utils.Core
{
    /// <summary>
    /// Draws a distinctive divider in the scene hierachy with changeable color. Used for grouping together objects in the hierarchy.
    /// </summary>
    public class HierarchyDivider : MonoBehaviour
    {
        public Color backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
    }
}