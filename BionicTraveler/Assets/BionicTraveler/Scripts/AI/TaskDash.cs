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
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDash"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TaskDash(DynamicEntity owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Dash;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.PlayAnimation("Dashing");
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            // TODO: Lerping movement to start fast, slow down.
            if (this.HasCurrentAnimationFinished())
            {
                this.End("We finished dashing", true);
            }
            else
            {
                var rb = this.Owner.GetComponent<Rigidbody2D>();
                rb.MovePosition(rb.position + (this.Owner.Direction.ToVector2() * 15 * Time.fixedDeltaTime));
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
