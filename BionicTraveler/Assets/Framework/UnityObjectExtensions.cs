namespace Framework
{
    /// <summary>
    /// Contains helper functions for <see cref="UnityEngine.Object"/>.
    /// </summary>
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// Checks if a <see cref="UnityEngine.Object"/> has been destroyed.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Whether or not the object has been destroyed.</returns>
        public static bool IsDestroyed(this UnityEngine.Object obj)
        {
            // Unity overloads this to allow checks for destroyed objects, but it feels _very_ wrong.
            return obj == null;
        }
    }
}
