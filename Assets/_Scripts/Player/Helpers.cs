using System.Collections.Generic;
using UnityEngine;

namespace Madkpv
{
    public static class Helpers
    {
        private static Dictionary<float, YieldInstruction> _cachedDelay = new Dictionary<float, YieldInstruction>();

        public static YieldInstruction CachedDelay(float delay)
        {
            if (_cachedDelay.TryGetValue(delay, out var yieldInstruction)) return yieldInstruction;
            yieldInstruction = new WaitForSeconds(delay);
            _cachedDelay.Add(delay, yieldInstruction);
            return yieldInstruction;
        }

        public static bool CheckLayerMask(this LayerMask layerMask, int layer) => (layerMask.value & (1 << layer)) > 0;

        public static float Remap(this float value, float min, float max, float mappedMin, float mappedMax)
        {
            var originalPos = Mathf.InverseLerp(min, max, value);
            return Mathf.Lerp(mappedMin,mappedMax,originalPos);
        }
    }
}