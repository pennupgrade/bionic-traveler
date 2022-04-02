namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Slow trap movement modifier that slows down an entity.
    /// </summary>
    public class SlowTrap : MonoBehaviour, IMovementModifier
    {

        [SerializeField]
        private AudioClip impactSound;

        /// <inheritdoc/>
        public float GetSpeedMultiplier(Entity entity)
        {
            return 0.3f;
        }

        public void Start()
        {
            AudioManager.Instance.PlayOneShot(impactSound);
        }

        /// <inheritdoc/>
        public bool CanStack()
        {
            return false;
        }
    }
}
