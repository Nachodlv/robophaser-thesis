using UnityEngine;

namespace Utils
{
    public static class Layers
    {
        public static bool Includes(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | 1 << layer);
        }

        public static bool Equals(this LayerMask layerMask, int layer)
        {
            return layerMask == 1 << layer;
        }
    }
}
