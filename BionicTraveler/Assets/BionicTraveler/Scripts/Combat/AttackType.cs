namespace BionicTraveler.Scripts.Combat
{
    /// <summary>
    /// Describes the type of an attack.
    /// </summary>
    public enum AttackType
    {
        /// <summary>
        /// A melee attack. Use this for instant damage to nearby targets.
        /// </summary>
        Melee,

        /// <summary>
        /// A ranged attack. Use this for delayed hit damage that spawns a physical projectile traversing
        /// the game world.
        /// </summary>
        RangedProjectile,

        /// <summary>
        /// A force field created around an entity.
        /// </summary>
        ForceField,
    }
}