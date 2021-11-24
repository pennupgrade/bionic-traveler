namespace BionicTraveler.Assets.Framework
{
    using UnityEngine;

    /// <summary>
    /// UI helper classes.
    /// </summary>
    public class UI
    {
        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="color">The color.</param>
        /// <param name="segments">The number of segments.</param>
        public static void DrawCircle(Vector2 center, float radius, Color color, float segments = 20f)
        {
            Quaternion segmentRot = Quaternion.AngleAxis(360.0f / segments, Vector3.forward);
            Vector2 circleStart = new Vector2(radius, 0.0f);
            for (int i = 0; i < segments; i++)
            {
                Vector2 rotatedPoint = segmentRot * circleStart;
                Debug.DrawLine(center + circleStart, center + rotatedPoint, color);
                circleStart = rotatedPoint;
            }
        }
    }
}
