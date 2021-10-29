namespace Framework
{
    using UnityEngine;

    /// <summary>
    /// Contains helper functions for <see cref="UnityEngine.Transform"/>.
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Returns the distance between two <see cref="UnityEngine.Transform"/>s.
        /// </summary>
        /// <param name="transform">The tranform.</param>
        /// <param name="otherTransform">The other tranform.</param>
        /// <returns>The distance to the other transform.</returns>
        public static float DistanceTo(this UnityEngine.Transform transform, UnityEngine.Transform otherTransform)
        {
            return Vector3.Distance(transform.position, otherTransform.position);
        }

        /// <summary>
        /// Returns whether a transform is within range of another transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="otherTransform">The other transform.</param>
        /// <param name="range">The range.</param>
        /// <returns>Whether the transform is in range.</returns>
        public static bool IsInRangeTo(this UnityEngine.Transform transform, UnityEngine.Transform otherTransform, float range)
        {
            return transform.DistanceTo(otherTransform) < range;
        }
    }
}