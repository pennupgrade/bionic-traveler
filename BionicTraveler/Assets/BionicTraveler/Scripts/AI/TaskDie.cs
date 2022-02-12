namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Basic dying task for an entity.
    /// </summary>
    public class TaskDie : TaskAnimated
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDash"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TaskDie(DynamicEntity owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Die;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            if (this.HasAnimation("Dying"))
            {
                this.PlayAnimation("Dying");
            }
            else
            {
                this.End("Entity has no dying animation", false);
            }
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (this.HasCurrentAnimationFinished())
            {
                this.End("We finished dying", true);
            }
            else
            {
                if (this.IsCurrentAnimationPlaying())
                {
                    
                }
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
