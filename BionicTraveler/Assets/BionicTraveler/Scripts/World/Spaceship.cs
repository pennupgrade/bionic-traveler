namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// The player's spaceship, which allows her to travel to other planets and charge her battery.
    /// </summary>
    public class Spaceship : DialogueObject
    {
        [Tooltip("The sound to play when the spaceship is being interacted with.")]
        [SerializeField]
        private AudioClip interactionSound;

        [Tooltip("The point where the player is spawned when they exit the ship.")]
        [SerializeField]
        private GameObject playerExitPoint;

        /// <summary>
        /// Gets the point where the player is spawned when they exit the ship.
        /// </summary>
        public GameObject PlayerExitPoint => this.playerExitPoint;

        /// <inheritdoc/>
        public override void OnInteract(GameObject obj)
        {
            Debug.Log("Interacted with Spaceship, healed Player");
            obj.GetComponent<PlayerEntity>()?.RestoreEnergy();
            AudioManager.Instance.PlayOneShot(this.interactionSound);
            //base.OnInteract(obj);
        }
    }
}
