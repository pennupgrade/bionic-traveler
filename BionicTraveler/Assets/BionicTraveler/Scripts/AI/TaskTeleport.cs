namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Task to go to a specific point.
    /// </summary>
    public class TaskTeleportAway : TaskAnimated
    {
        private Vector3 targetPos;
        private GameTime teleportTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTeleportAway"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="targetPos">The position to go to.</param>
        public TaskTeleportAway(DynamicEntity owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Teleport;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.teleportTime = GameTime.Now;
            this.PlayAnimation("TeleportAway");
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (!this.IsCurrentAnimationPlaying())
            {
                Vector3 tgt = new Vector3(Random.Range(4f, 5.0f), 0f, 0f);
                float rand = Random.Range(0f, 360.0f);
                tgt = Quaternion.Euler(0f, 0f, rand) * tgt;
                this.Owner.transform.position = this.Owner.transform.position + tgt;
                this.End("Teleported Away", true);
            }
        }
    }
}
