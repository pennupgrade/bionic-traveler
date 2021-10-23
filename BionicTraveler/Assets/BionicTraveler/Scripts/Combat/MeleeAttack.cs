namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.Items;

    /// <summary>
    /// Describes an instant melee attack.
    /// </summary>
    public class MeleeAttack : Attack
    {
        private bool hasAttacked;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            // TODO: Get targets in range.
            return new Entity[0];
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            this.hasAttacked = true;
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasAttacked;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }
    }
}
