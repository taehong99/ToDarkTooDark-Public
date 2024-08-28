using UnityEngine;

namespace JH
{
    public static class Extention
    {
        public static bool Contain(this LayerMask layerMask, int layer)
        {
            return ((1 << layer) & layerMask) != 0;
        }
    }
}
