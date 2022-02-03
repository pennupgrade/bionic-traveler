namespace BionicTraveler.Scripts.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Task to follow a number of waypoints.
    /// </summary>
    public class TaskFollowWaypoints : EntityTask
    {
        private int waypointIndex; // Progress so far in the movement
        private List<Vector3> waypoints;
        private TaskGoToPoint currentMovementTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFollowWaypoints"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="waypoints">The waypoints.</param>
        public TaskFollowWaypoints(DynamicEntity owner, IEnumerable<Vector3> waypoints)
            : base(owner)
        {
            this.waypoints = waypoints.ToList();
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.FollowWaypoints;

        public override void OnInitialize()
        {
            if (this.waypoints.Count == 0)
            {
                return;
            }

            this.currentMovementTask = new TaskGoToPoint(this.Owner, this.waypoints[0]);
            this.Owner.TaskManager.Assign(this.currentMovementTask);
        }

        public override void OnProcess()
        {
            if (this.waypoints.Count == 0)
            {
                this.End("No more waypoints", true);
                return;
            }

            if (this.currentMovementTask.HasEnded)
            {
                Debug.Log("Assign new Task");
                this.waypointIndex++;
                if (this.waypointIndex >= this.waypoints.Count)
                {
                    this.End("No more waypoints", true);
                    return;
                }

                this.currentMovementTask = new TaskGoToPoint(this.Owner, this.waypoints[this.waypointIndex]);
                this.Owner.TaskManager.Assign(this.currentMovementTask);
            }
        }
    }
}