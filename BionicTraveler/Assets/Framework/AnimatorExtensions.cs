namespace Framework
{
    using UnityEngine;

    /// <summary>
    /// Contains helper functions for collections.
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Returns whether <paramref name="name"/> exists as an animation.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether the animation exists.</returns>
        public static bool HasAnimation(this Animator animator, string name)
        {
            var animHash = Animator.StringToHash(name);
            return animator.HasState(0, animHash);
        }

        /// <summary>
        /// Returns whether <paramref name="name"/> has finished playing.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether animation has finished playing.</returns>
        public static bool HasAnimationFinished(this Animator animator, string name)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(name) && state.normalizedTime >= 1;
        }

        /// <summary>
        /// Returns whether <paramref name="name"/> is currently playing.
        /// </summary>
        /// <param name="animator">The animator.</param>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether animation is currently playing.</returns>
        public static bool IsAnimationPlaying(this Animator animator, string name)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(name) && state.normalizedTime < 1;
        }
    }
}
