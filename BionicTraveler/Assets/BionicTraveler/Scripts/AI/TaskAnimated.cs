namespace BionicTraveler.Scripts.AI
{
    using System;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Abstract base class for tasks that animate an entity.
    /// </summary>
    public abstract class TaskAnimated : EntityTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAnimated"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TaskAnimated(DynamicEntity owner)
            : base(owner)
        {
            this.Animator = this.Owner.GetComponent<Animator>();
        }

        /// <summary>
        /// Gets the animator of the entity.
        /// </summary>
        protected Animator Animator { get; }

        /// <summary>
        /// Gets the animation that is currently playing. Only works for animations started via
        /// <see cref="this.PlayAnimation"/>.
        /// </summary>
        protected string CurrentAnimation { get; private set; }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Animated;

        /// <summary>
        /// Plays a new animation.
        /// </summary>
        /// <param name="animationName">The animation to play.</param>
        public void PlayAnimation(string animationName)
        {
            this.CurrentAnimation = animationName;
            this.Animator.Play(animationName);
        }

        /// <summary>
        /// Returns whether the current animation has finished playing. Only works for animations started via
        /// <see cref="this.PlayAnimation"/>.
        /// </summary>
        /// <returns>Whether animation has finished playing.</returns>
        public bool HasCurrentAnimationFinished()
            => this.HasAnimationFinished(this.CurrentAnimation);

        /// <summary>
        /// Returns whether <paramref name="name"/> has finished playing.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether animation has finished playing.</returns>
        public bool HasAnimationFinished(string name)
        {
            var state = this.Animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(name) && state.normalizedTime >= 1;
        }

        /// <summary>
        /// Returns whether the current animation is currently playing. Only works for animations started via
        /// <see cref="this.PlayAnimation"/>.
        /// </summary>
        /// <returns>Whether animation is currently playing.</returns>
        public bool IsCurrentAnimationPlaying()
            => this.IsAnimationPlaying(this.CurrentAnimation);

        /// <summary>
        /// Returns whether <paramref name="name"/> is currently playing.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether animation is currently playing.</returns>
        public bool IsAnimationPlaying(string name)
        {
            var state = this.Animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(name) && state.normalizedTime < 1;
        }

        /// <summary>
        /// Returns whether <paramref name="name"/> exists as an animation.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether the animation exists.</returns>
        public bool HasAnimation(string name)
        {
            var animHash = Animator.StringToHash(name);
            return this.Animator.HasState(0, animHash);
        }

        /// <summary>
        /// Updates all animator values centrally. Called via <see cref="OnProcess"/> base.
        /// </summary>
        public virtual void UpdateAnimator()
        {
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            this.UpdateAnimator();
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            base.OnEnd();
            this.Owner.ResetAnimationState();
        }
    }
}