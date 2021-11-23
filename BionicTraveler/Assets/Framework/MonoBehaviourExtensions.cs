namespace Framework
{
    using UnityEngine;

    /// <summary>
    /// Contains helper functions for <see cref="UnityEngine.MonoBehaviour"/>.
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Disables this behaviour.
        /// </summary>
        /// <param name="behaviour">The behaviour.</param>
        public static void Disable(this MonoBehaviour behaviour)
        {
            behaviour.enabled = false;
        }
    }
}