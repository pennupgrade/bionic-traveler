namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;

    /// <summary>
    /// Task to follow a <see cref="DynamicEntity"/>.
    /// </summary>
    public class TaskFollowEntity : EntityTask
    {
        private Entity targetEntity;
        private GameTime lastPosUpdate;
        private EntityMovement movement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFollowEntity"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entity">The entity to follow.</param>
        public TaskFollowEntity(DynamicEntity owner, Entity entity)
            : this(owner, entity, 1.0f)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFollowEntity"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entity">The entity to follow.</param>
        /// <param name="distance">The distance to maintain.</param>
        public TaskFollowEntity(DynamicEntity owner, Entity entity, float distance)
            : base(owner)
        {
            this.targetEntity = entity;
            this.lastPosUpdate = GameTime.Default;
            this.movement = this.Owner.GetComponent<EntityMovement>();
            this.movement.StopDistance = distance;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.FollowEntity;

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
