using UnityEngine;

namespace Utils.Core.Extensions
{
    public static class LayerMaskExtensions 
    {
        public static bool LayerMaskContainsLayer(LayerMask layerMask, GameObject obj)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }

        public static bool LayerMaskContainsLayer(LayerMask layerMask, int layer)
        {
            return ((layerMask.value & (1 << layer)) > 0);
        }

        public static bool ContainsLayer(this LayerMask layerMask, int layer)
        {
            return LayerMaskContainsLayer(layerMask, layer);
        }
    }
}