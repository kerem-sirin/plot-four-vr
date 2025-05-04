using UnityEngine;

namespace PlotFourVR
{
    public static class GridLayoutService
    {
        public const float Spacing = 0.2f;
        public static Vector3 ComputeGridOffset(int cols) => new Vector3(-((cols - 1) * Spacing) / 2f, 0.5f, 2f);
    }
}