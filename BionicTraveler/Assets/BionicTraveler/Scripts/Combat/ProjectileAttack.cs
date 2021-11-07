namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;

    /// <summary>
    /// Describes a ranged attack that creates a projectile.
    /// </summary>
    public class ProjectileAttack : Attack
    {
        private bool hasImpacted;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            // TODO: Get targets in range or maybe defer to collision event?
            return new Entity[0];
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            // TODO: Fire projectile and wait for impact.
            this.hasImpacted = true;
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasImpacted;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            // TODO: Dispose projectile.
        }
    }
}
