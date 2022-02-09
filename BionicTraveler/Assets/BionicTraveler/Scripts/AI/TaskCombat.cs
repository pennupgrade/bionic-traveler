namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using Framework;

    class TaskCombat : EntityTask
    {
        private readonly Entity target;

        public override EntityTaskType Type => EntityTaskType.Combat;

        public float MinimumDistance { get; set; }

        private TaskFollowEntity followTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCombat"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="target">The target.</param>
        public TaskCombat(DynamicEntity owner, Entity target)
            : base(owner)
        {
            this.target = target;
        }

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            base.OnInitialize();
            this.AssignFollowTask();
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            // TODO: Read range from weapon.
            var distanceToTarget = this.Owner.transform.DistanceTo(this.target.transform);
            if (distanceToTarget < 5f)
            {
                if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Attack))
                {
                    var attack = new TaskAttack(this.Owner, true);
                    attack.Assign();
                }
            }
            else
            {
                if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Attack) &&
                    !this.Owner.TaskManager.IsTaskActive(EntityTaskType.FollowEntity))
                {
                    this.AssignFollowTask();
                }
            }
        }

        private void AssignFollowTask()
        {
            this.followTask = this.MinimumDistance > 0
               ? new TaskFollowEntity(this.Owner, this.target, this.MinimumDistance)
               : new TaskFollowEntity(this.Owner, this.target);
            this.followTask.Assign();
        }
    }
}