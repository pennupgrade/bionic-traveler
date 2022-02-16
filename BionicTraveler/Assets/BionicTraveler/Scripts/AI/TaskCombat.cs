namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Main combat task that manages launching attacks again a target.
    /// </summary>
    public class TaskCombat : EntityTask
    {
        private readonly Entity target;
        private readonly WeaponBehaviour weaponBehavior;
        private bool usePrimaryWeaponMode;
        private AttackData attackData;
        private TaskAttack taskAttack;
        private EntityTask taskMovement;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCombat"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="target">The target.</param>
        public TaskCombat(DynamicEntity owner, Entity target)
            : base(owner)
        {
            this.target = target;
            this.weaponBehavior = this.Owner.WeaponsInventory.equippedWeaponBehavior;
            this.usePrimaryWeaponMode = true;
            this.weaponBehavior.SetWeaponMode(this.usePrimaryWeaponMode);
            this.Owner.WeaponsInventory.DisplayCurrentWeapon();
            this.attackData = this.weaponBehavior.GetNextAttackData();
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Combat;

        /// <summary>
        /// Gets or sets the stopping distance used when moving towards the target.
        /// </summary>
        public float FollowStoppingDistance { get; set; }

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("TaskCombat");
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            // Ignore dead target.
            if (this.target.IsDeadOrDying)
            {
                this.End("Target is dead or dying", true);
                return;
            }

            // Move away from target, if we are too close.
            var distanceToTarget = this.Owner.transform.DistanceTo(this.target.transform);
            if (distanceToTarget < this.attackData.MinimumAggressiveRange)
            {
                if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.MoveFromEntity))
                {
                    this.taskAttack?.End("Target too close", false);
                    this.taskMovement = new TaskMoveFromEntity(this.Owner, this.target, this.attackData.MinimumAggressiveRange + 2f);
                    this.taskMovement.Assign();
                }
            }
            else if (distanceToTarget < this.attackData.MaximumAggressiveRange)
            {
                if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Attack))
                {
                    // Ensure our weapon is ready to be fired before we assign this task.
                    if (this.weaponBehavior.IsReady(this.Owner))
                    {
                        this.taskMovement?.End("We can attack now", false);
                        this.taskAttack = new TaskAttack(this.Owner, true);
                        this.taskAttack.Assign();
                    }
                }
            }
            else
            {
                if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Attack) &&
                    !this.Owner.TaskManager.IsTaskActive(EntityTaskType.MoveToEntity))
                {
                    this.AssignFollowTask();
                }
            }
        }

        private void AssignFollowTask()
        {
            this.taskMovement = this.FollowStoppingDistance > 0
               ? new TaskMoveToEntity(this.Owner, this.target, this.FollowStoppingDistance)
               : new TaskMoveToEntity(this.Owner, this.target);
            this.taskMovement.Assign();
        }
    }
}