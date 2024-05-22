#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEditor;

namespace Utils.Core.SerializedSubClass
{
    public static class SerializedPropertyExtensions
    {
        public static IEnumerable<SerializedProperty> GetChildProperties(this SerializedProperty parent, int depth = 1)
        {
            parent = parent.Copy();

            int depthOfParent = parent.depth;
            var enumerator = parent.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is SerializedProperty childProperty))
                {
                    continue;
                }
                if (childProperty.depth > (depthOfParent + depth))
                {
                    continue;
                }
                yield return childProperty.Copy();
            }
        }
    }
}
#endif