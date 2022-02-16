namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes the task type.
    /// </summary>
    public enum EntityTaskType
    {
        GoToPoint,
        MoveToEntity,
        PickUpItem,
        Patrol,
        FollowWaypoints,
        PlayerMovement,
        Animated,
        Dash,
        ExecuteSequence,
        Attack,
        Combat,
        Projectile,
        Die,
        MoveFromEntity
    }

    /// <summary>
    /// Abstract base class for tasks that can be assigned to entities.
    /// </summary>
    public abstract class EntityTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTask"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public EntityTask(DynamicEntity owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Delegate describing a task has ended.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="successful">Whether the task ended successfully.</param>
        public delegate void EntityTaskEndedEventHandler(EntityTask task, bool successful);

        /// <summary>
        /// Fired when the task has ended.
        /// </summary>
        public event EntityTaskEndedEventHandler Ended;

        /// <summary>
        /// Gets the owner of the task.
        /// </summary>
        public DynamicEntity Owner { get; }

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        public abstract EntityTaskType Type { get; }

        /// <summary>
        /// Gets a value indicating whether this task has been initialized, i.e. it has ticked at least once.
        /// </summary>
        public bool WasInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this task has been cancelled, i.e. it has ended but not successfully.
        /// </summary>
        public bool WasCancelled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this task has ended successfully.
        /// </summary>
        public bool WasSuccessful { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task has ended.
        /// </summary>
        public bool HasEnded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this task is currently active.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this task can run even after an enttiy has died.
        /// </summary>
        public bool PersistAfterDeath { get; protected set; }

        /// <summary>
        /// Assigns the task to <see cref="owner"/>.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Assign()
        {
            this.Owner.TaskManager.Assign(this);
        }

        /// <summary>
        /// Called when this task is being initialized, i.e. when it just got assigned and the first time its <see cref="Process"/> is about to be called.
        /// Guaranteed to only be called once.
        /// </summary>
        public virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Initializes the task.
        /// </summary>
        public void Initialize()
        {
            this.IsActive = true;
            this.WasInitialized = true;
            this.OnInitialize();
        }

        /// <summary>
        /// Called when the task ticks, i.e. it is being processed.
        /// </summary>
        public abstract void OnProcess();

        /// <summary>
        /// Processes aka ticks the task logic.
        /// </summary>
        public void Process()
        {
            if (this.Owner.IsDeadOrDying && !this.PersistAfterDeath)
            {
                this.End("Owner died", false);
                return;
            }

            this.OnProcess();
        }

        /// <summary>
        /// Called when the task has ended.
        /// </summary>
        public virtual void OnEnd()
        {
        }

        /// <summary>
        /// Ends the task with a given reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="successful">Whether the task was successful.</param>
        public void End(string reason, bool successful)
        {
            if (!this.IsActive)
            {
                return;
            }

            Debug.Log(reason);
            this.End(successful);
        }

        private void End(bool successful)
        {
            this.IsActive = false;
            this.WasSuccessful = successful;
            this.WasCancelled = !successful;
            this.HasEnded = true;

            // In case we need shared logic for ending between all types
            this.OnEnd();

            this.Ended?.Invoke(this, this.WasSuccessful);
        }
    }
}
