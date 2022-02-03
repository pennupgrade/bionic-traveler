namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Basic dash task for an entity.
    /// </summary>
    public class TaskDash : TaskAnimated
    {
        private float initialSpeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDash"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TaskDash(DynamicEntity owner)
            : base(owner)
        {
            this.initialSpeed = 15f;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Dash;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            if (this.HasAnimation("Dashing"))
            {
                this.PlayAnimation("Dashing");
            }
            else
            {
                this.End("Entity has no dashing animation", false);
            }
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (this.HasCurrentAnimationFinished())
            {
                this.End("We finished dashing", true);
            }
            else
            {
                if (this.IsCurrentAnimationPlaying())
                {
                    var rb = this.Owner.GetComponent<Rigidbody2D>();
                    var state = this.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    var speed = Mathf.Lerp(this.initialSpeed, 0, state);
                    rb.MovePosition(rb.position + (this.Owner.Direction.ToVector2() * speed * Time.fixedDeltaTime));
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
