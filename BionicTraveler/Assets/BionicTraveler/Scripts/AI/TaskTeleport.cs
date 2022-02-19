namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Assets.Framework;

    /// <summary>
    /// Task to go to a specific point.
    /// </summary>
    public class TaskTeleportAway : EntityTask
    {
        private Vector3 targetPos;
        private GameTime teleportTime;
        private Animator animator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskGoToPoint"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="targetPos">The position to go to.</param>
        public TaskTeleportAway(DynamicEntity owner)
            : base(owner)
        {
            //Teleport Away

            this.animator = this.Owner.GetComponent<Animator>();
            animator.Play("TeleportAway");

            //Teleport Back
        }

        public override EntityTaskType Type => EntityTaskType.Teleport;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            //this.movement.StopDistance = 1.0f;
            //this.movement.SetTarget(this.targetPos);
            //this.Owner.transform.position = targetPos;
            teleportTime = GameTime.Now;
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            //Debug.DrawLine(this.Owner.transform.position, this.targetPos);

            if (teleportTime.HasTimeElapsed(.45f))
            {
                Vector3 tgt = new Vector3(Random.Range(4f, 5.0f), 0f, 0f);
                float rand = Random.Range(0f, 360.0f);
                tgt = Quaternion.Euler(0f, 0f, rand) * tgt;
                Owner.transform.position = Owner.transform.position + tgt;
                this.End("Teleported Away", true);
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            // No movement to clean up
            
        }
    }
}
