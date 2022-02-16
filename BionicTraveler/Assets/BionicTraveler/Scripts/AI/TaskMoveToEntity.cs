namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;

    /// <summary>
    /// Task to move to a <see cref="DynamicEntity"/>.
    /// </summary>
    public class TaskMoveToEntity : EntityTask
    {
        private Entity targetEntity;
        private GameTime lastPosUpdate;
        private EntityMovement movement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMoveToEntity"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entity">The entity to follow.</param>
        public TaskMoveToEntity(DynamicEntity owner, Entity entity)
            : this(owner, entity, 1.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMoveToEntity"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entity">The entity to follow.</param>
        /// <param name="distance">The distance to maintain.</param>
        public TaskMoveToEntity(DynamicEntity owner, Entity entity, float distance)
            : base(owner)
        {
            this.targetEntity = entity;
            this.lastPosUpdate = GameTime.Default;
            this.movement = this.Owner.GetComponent<EntityMovement>();
            this.movement.StopDistance = distance;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.MoveToEntity;

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (this.lastPosUpdate.HasTimeElapsedReset(0.5f))
            {
                this.movement.SetTarget(this.targetEntity.transform.position);
            }

            if (this.movement.HasReached)
            {
                this.End("Target Reached", true);
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            // Remove target from movement.
            this.movement.ClearTarget();
        }
    }
}
