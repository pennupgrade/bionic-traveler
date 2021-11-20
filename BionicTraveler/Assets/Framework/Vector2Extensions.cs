namespace Framework
{
    using UnityEngine;

    static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            return Quaternion.Euler(0, 0, degrees) * v;
        }

        /// <summary>
        /// Returns the <see cref="Vector3"/> reprensentation of this 2D vector.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The 3D Vector.</returns>
        public static Vector3 ToVector3(this Vector2 position)
        {
            return new Vector3(position.x, position.y, 0);
        }
    }
}