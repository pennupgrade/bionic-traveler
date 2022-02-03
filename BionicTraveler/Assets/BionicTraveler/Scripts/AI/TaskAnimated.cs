namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BionicTraveler.Scripts.World;
    using Framework;
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
        /// <returns>Whether animation has finsihed playing.</returns>
        public bool HasCurrentAnimationFinished()
            => this.HasAnimationFinished(this.CurrentAnimation);

        /// <summary>
        /// Returns whether <paramref name="name"/> has finished playing.
        /// </summary>
        /// <param name="name">The name of the animation.</param>
        /// <returns>Whether animation has finsihed playing.</returns>
        public bool HasAnimationFinished(string name)
        {
            var state = this.Animator.GetCurrentAnimatorStateInfo(0);
            return state.IsName(name) && state.normalizedTime >= 1;
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
    }
}