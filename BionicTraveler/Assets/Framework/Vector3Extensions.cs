namespace Framework
{
    using UnityEngine;

    /// <summary>
    /// Contains helper functions for <see cref="UnityEngine.Vector3"/>.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Returns a new <see cref="Vector3"/> with its Y component changed by <paramref name="offset"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The new vector.</returns>
        public static Vector3 GetOffsetY(this Vector3 position, float offset)
        {
            return position + (Vector3.up * offset);
        }
    }
}