namespace BionicTraveler.Scripts.AI
{
    using System.Collections.Generic;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Task that executes a <see cref="TaskSequence"/>.
    /// </summary>
    public class TaskExecuteSequence : EntityTask
    {
        private readonly TaskSequence sequence;
        private EntityTask[] tasks;
        private int sequenceIndex;
        private EntityTask currentTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskExecuteSequence"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="sequence">The sequence.</param>
        public TaskExecuteSequence(DynamicEntity owner, TaskSequence sequence)
            : base(owner)
        {
            this.sequence = sequence;
            this.tasks = this.sequence.Tasks;
            this.sequenceIndex = -1;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.ExecuteSequence;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.AssignNextTask();
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
        }

        private bool AssignNextTask()
        {
            this.sequenceIndex++;
            if (this.sequenceIndex >= this.tasks.Length)
            {
                this.End("Sequence has finished", true);
                return false;
            }

            if (this.currentTask != null)
            {
                this.currentTask.Ended -= this.CurrentTask_Ended;
            }

            this.currentTask = this.tasks[this.sequenceIndex];
            this.currentTask.Ended += this.CurrentTask_Ended;
            this.currentTask.Assign();
            return true;
        }

        private void CurrentTask_Ended(EntityTask task, bool successful)
        {
            Debug.Log("Starting next sequence task");
            this.AssignNextTask();
        }
    }
}