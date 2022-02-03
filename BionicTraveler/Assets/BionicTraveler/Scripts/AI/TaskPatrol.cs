namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes the patrol type.
    /// </summary>
    public enum PatrolType
    {
        Square,
    }

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TaskPatrol : EntityTask
    {
        private PatrolType type;
        private TaskFollowWaypoints followTask;
        private List<Vector3> waypoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPatrol"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="type">The type.</param>
        public TaskPatrol(DynamicEntity owner, PatrolType type)
            : base(owner)
        {
            this.type = type;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Patrol;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.waypoints = new List<Vector3>();
            if (this.type == PatrolType.Square)
            {
                // Add 4 points to waypoints.
                this.waypoints.Add(this.Owner.transform.position + new Vector3(-5, -5, 0));
                this.waypoints.Add(this.Owner.transform.position + new Vector3(-5, 5, 0));
                this.waypoints.Add(this.Owner.transform.position + new Vector3(5, 5, 0));
                this.waypoints.Add(this.Owner.transform.position + new Vector3(5, -5, 0));
            }

            Debug.Log("Assign first task");
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
           if (this.followTask == null || this.followTask.HasEnded)
            {
                this.followTask = new TaskFollowWaypoints(this.Owner, this.waypoints);
                this.Owner.TaskManager.Assign(this.followTask);
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            if (this.followTask != null)
            {
                this.followTask.End("Parent task shutting down...", this.WasSuccessful);
            }
        }
    }
}
