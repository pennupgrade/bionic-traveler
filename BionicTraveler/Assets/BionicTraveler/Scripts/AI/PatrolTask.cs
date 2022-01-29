namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum PatrolType
    {
        Square,
    }
    /// <summary>
    /// Please document me.
    /// </summary>
    public class PatrolTask : EntityTask
    {
        private PatrolType type;
        private int waypointIndex; // Progress so far in the movement
        private List<Vector3> waypoints;
        private GoToPointTask currentMovementTask;

        private Vector3 center;
        public PatrolTask(DynamicEntity owner, PatrolType type)
            : base(owner)
        {
            this.type = type;
            this.center = owner.transform.position;
            this.waypoints = new List<Vector3>();
        }

        public override EntityTaskType Type => EntityTaskType.Patrol;

        public override void OnInitialize()
        {
            if (this.type == PatrolType.Square)
            {
                // add 4 points to waypoints
                this.waypoints.Add(new Vector3(-5, -5, 0));
                this.waypoints.Add(new Vector3(-5, 5, 0));
                this.waypoints.Add(new Vector3(5, 5, 0));
                this.waypoints.Add(new Vector3(5, -5, 0));
            }

            this.currentMovementTask = new GoToPointTask(this.Owner, this.waypoints[0] + this.center);
            this.Owner.TaskManager.Assign(this.currentMovementTask);

            Debug.Log("Assign first task");
        }

        public override void OnProcess()
        {
            if (this.currentMovementTask.HasEnded)
            {
                Debug.Log("Assign new Task");
                this.waypointIndex++;
                if (this.waypointIndex >= this.waypoints.Count)
                {
                    this.waypointIndex = 0;
                }
                this.currentMovementTask = new GoToPointTask(this.Owner, this.waypoints[this.waypointIndex] + this.center);
                this.Owner.TaskManager.Assign(this.currentMovementTask);

            }
        }

        public override void OnEnd()
        {
            if (this.currentMovementTask != null)
            {
                this.currentMovementTask.End("Parent task shutting down...");
            }
        }
    }
}
