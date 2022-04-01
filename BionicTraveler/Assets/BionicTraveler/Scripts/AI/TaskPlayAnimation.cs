namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Basic dash task for an entity.
    /// </summary>
    public class TaskPlayAnimation : TaskAnimated
    {
        private string animName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPlayAnimation"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="animationName">The animation name.</param>
        public TaskPlayAnimation(DynamicEntity owner, string animationName)
            : base(owner)
        {
            this.animName = animationName;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.PlayAnimation;

        /// <summary>
        /// Gets the normalized progress of the animation.
        /// </summary>
        public float Progress => this.Animator.GetAnimationProgress(this.animName);

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            if (this.HasAnimation(this.animName))
            {
                this.PlayAnimation(this.animName);
            }
            else
            {
                this.End("Entity has no " + this.animName + " animation", false);
            }
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (this.HasCurrentAnimationFinished())
            {
                this.End("We finished playing anim", true);
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
