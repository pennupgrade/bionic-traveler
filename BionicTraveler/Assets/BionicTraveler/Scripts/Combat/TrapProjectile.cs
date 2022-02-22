namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TrapProjectile : Attack
    {
        private bool isOutOfRange;
        private Vector3 origin;
        private float throwDistanceRand;
        private bool hasSpawned;

        public GameObject stickyTrap;

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
        }

        /// <inheritdoc/>
        public override void OnAttackStarted()
        {
            base.OnAttackStarted();

            this.origin = this.transform.position;
            this.throwDistanceRand = UnityEngine.Random.Range(5, 10);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            return new Entity[0];
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.isOutOfRange;
        }

        private void FixedUpdate()
        {
            float distanceTraveled = (this.transform.position - this.origin).magnitude;
            if (distanceTraveled >= this.throwDistanceRand && !this.hasSpawned)
            {
                // Generate Sticky trap object
                Vector3 spawnPos = this.transform.position;

                GameObject stickyTrap = Instantiate(this.stickyTrap, spawnPos, Quaternion.identity);
                this.isOutOfRange = true;
                this.hasSpawned = true;
            }
        }
    }
}
