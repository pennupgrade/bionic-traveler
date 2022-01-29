namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class FollowEntityTask : EntityTask
    {

        private Entity targetEntity;
        private GameTime lastPosUpdate;
        private EntityMovement movement;
        public FollowEntityTask(DynamicEntity owner, Entity entity)
            : base(owner)
        {
            this.targetEntity = entity;
            this.lastPosUpdate = GameTime.Default;
            this.movement = this.Owner.GetComponent<EntityMovement>();
        }

        public override EntityTaskType Type => EntityTaskType.FollowEntity;

        public override void OnProcess()
        {
            if (this.lastPosUpdate.HasTimeElapsedReset(0.5f))
            {
                this.movement.SetTarget(this.targetEntity.transform.position);

            }
            
            if (this.movement.HasReached)
            {
                this.End("Target Reached");
            }
        }

        public override void OnEnd()
        {
            // Removes target from Movement
            this.movement.ClearTarget();
        }
    }
}
