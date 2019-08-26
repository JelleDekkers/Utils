using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Removes all child transforms
        /// </summary>
        /// <param name="t"></param>
        public static void RemoveAllChildren(this Transform t)
        {
            foreach (Transform child in t.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
