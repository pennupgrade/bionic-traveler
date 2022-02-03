
namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// A sequence of tasks that are executed in order.
    /// </summary>
    public class TaskSequence
    {
        private List<EntityTask> tasks;
        private bool hasStarted;
        private TaskExecuteSequence sequenceTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSequence"/> class.
        /// </summary>
        public TaskSequence()
        {
            this.tasks = new List<EntityTask>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSequence"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskSequence(EntityTask task)
            : this()
        {
            this.AddTask(task);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSequence"/> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        public TaskSequence(IEnumerable<EntityTask> tasks)
            : this()
        {
            this.AddTasks(tasks);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSequence"/> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        public TaskSequence(params EntityTask[] tasks)
            : this(tasks.ToList())
        {
        }

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        public EntityTask[] Tasks => this.tasks.ToArray();

        /// <summary>
        /// Adds the task to the sequence.
        /// </summary>
        /// <param name="task">The task.</param>
        public void AddTask(EntityTask task)
        {
            this.ThrowIfRunning();
            this.tasks.Add(task);
        }

        /// <summary>
        /// Adds the tasks to the sequence.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        public void AddTasks(IEnumerable<EntityTask> tasks)
        {
            this.ThrowIfRunning();
            this.tasks.AddRange(tasks);
        }

        /// <summary>
        /// Adds the tasks to the sequence.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        public void AddTasks(params EntityTask[] tasks)
        {
            this.ThrowIfRunning();
            this.tasks.AddRange(tasks);
        }

        /// <summary>
        /// Executes the task sequence for the given owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Execute(DynamicEntity owner)
        {
            this.ThrowIfRunning();
            this.hasStarted = true;
            this.sequenceTask = new TaskExecuteSequence(owner, this);
            this.sequenceTask.Assign();
        }

        private void ThrowIfRunning()
        {
            if (this.hasStarted)
            {
                throw new InvalidOperationException("Cannot add tasks to running sequence.");
            }
        }
    }
}