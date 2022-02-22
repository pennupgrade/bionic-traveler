namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Slow trap movement modifier that slows down an entity.
    /// </summary>
    public class SlowTrap : MonoBehaviour, IMovementModifier
    {
        /// <inheritdoc/>
        public float GetSpeedMultiplier(Entity entity)
        {
            return 0.3f;
        }

        /// <inheritdoc/>
        public bool CanStack()
        {
            return false;
        }
    }
}
