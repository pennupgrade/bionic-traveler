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

        /// <inheritdoc/>
        public override void OnInteract(GameObject obj)
        {
            Debug.Log("Interacted with Spaceship, healed Player");
            obj.GetComponent<PlayerEntity>()?.HealBattery();
            AudioManager.Instance.PlayOneShot(this.interactionSound);
            //base.OnInteract(obj);
        }
    }
}
