using UnityEngine;

namespace PlotFourVR.Helpers
{
    /// <summary>
    /// Provides utility method(s) for calculating grid layout offsets.
    /// </summary>
    public static class GridLayoutService
    {
        public const float Spacing = 0.2f;
        public static Vector3 ComputeGridOffset(int rows, int cols)
        {
            // Calculate the offset based on the number of rows and columns
            // Magical numbers are min&max values for the row count
            float heightNormalized = Mathf.InverseLerp(5, 10, rows);
            float yPos = Mathf.Lerp(1f, 0.5f, heightNormalized);
            return new Vector3(-((cols - 1) * Spacing) / 2f, yPos, 2f); ;
        }
    }
}