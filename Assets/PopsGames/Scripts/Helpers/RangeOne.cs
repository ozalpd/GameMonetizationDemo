using UnityEngine;

namespace Pops.Helpers
{
    /// <summary>
    /// Range info for one dimension.
    /// </summary>
    public class RangeOne
    {
        public RangeOne(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Min { get; private set; }
        public float Max { get; private set; }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
    }
}