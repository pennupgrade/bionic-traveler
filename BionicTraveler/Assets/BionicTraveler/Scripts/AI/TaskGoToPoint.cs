namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Task to go to a specific point.
    /// </summary>
    public class TaskGoToPoint : EntityTask
    {
        private Vector3 targetPos;
        private EntityMovement movement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskGoToPoint"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="targetPos">The position to go to.</param>
        public TaskGoToPoint(DynamicEntity owner, Vector3 targetPos)
            : base(owner)
        {
            this.targetPos = targetPos;
            this.movement = this.Owner.GetComponent<EntityMovement>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether a slower movement speed is used, i.e. walking.
        /// </summary>
        public bool ForceWalking
        {
            get => this.movement.ForceWalking;
            set => this.movement.ForceWalking = value;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.GoToPoint;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.movement.StopDistance = 1.0f;
            this.movement.SetTarget(this.targetPos);
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            Debug.DrawLine(this.Owner.transform.position, this.targetPos);
            if (this.movement.HasReached)
            {
                this.End("Target Position Reached", true);
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            // Removes target from Movement
            this.movement.ClearTarget();
        }
    }
}
