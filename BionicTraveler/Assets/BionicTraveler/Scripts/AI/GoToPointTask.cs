namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class GoToPointTask : EntityTask
    {
        private Vector3 targetPos;
        private EntityMovement movement;
        public override EntityTaskType Type => EntityTaskType.GoToPoint;

        public GoToPointTask(DynamicEntity owner, Vector3 targetPos)
            : base(owner)
        {
            this.targetPos = targetPos;
            this.movement = this.Owner.GetComponent<EntityMovement>();
        }

        public override void OnInitialize()
        {
            this.movement.SetTarget(this.targetPos);
        }
        public override void OnProcess()
        {
            Debug.DrawLine(this.Owner.transform.position, this.targetPos);
            if (this.movement.HasReached)
            {
                this.End("Target Position Reached");
            }
        }

        public override void OnEnd()
        {
            // Removes target from Movement
            this.movement.ClearTarget();
        }
    }
}
