namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Task to move away from a <see cref="DynamicEntity"/>.
    /// </summary>
    public class TaskMoveFromEntity : EntityTask
    {
        private Entity targetEntity;
        private float distance;
        private GameTime lastPosUpdate;
        private EntityMovement movement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskMoveFromEntity"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="entity">The entity to follow.</param>
        /// <param name="distance">The distance to move away.</param>
        public TaskMoveFromEntity(DynamicEntity owner, Entity entity, float distance)
            : base(owner)
        {
            this.targetEntity = entity;
            this.distance = distance;
            this.lastPosUpdate = GameTime.Default;
            this.movement = this.Owner.GetComponent<EntityMovement>();
            this.movement.StopDistance = 1.5f;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.MoveFromEntity;

        /// <inheritdoc/>
        public override void OnProcess()
        {
            var distanceToTarget = this.Owner.transform.DistanceTo(this.targetEntity.transform);
            if (distanceToTarget >= this.distance)
            {
                this.End("Far away enough", true);
                return;
            }

            if (this.lastPosUpdate.HasTimeElapsedReset(0.5f))
            {
                // Calculate position away from target.
                var directionAway = (this.Owner.transform.position - this.targetEntity.transform.position).normalized;
                var pointAway = this.targetEntity.transform.position + (directionAway * (this.distance + this.movement.StopDistance));
                Debug.DrawLine(this.Owner.transform.position, pointAway, Color.red, 0.5f);

                this.movement.SetTarget(pointAway);
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
