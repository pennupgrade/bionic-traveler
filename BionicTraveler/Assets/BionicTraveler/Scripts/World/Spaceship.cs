namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.Quests;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// The player's spaceship, which allows her to travel to other planets and charge her battery.
    /// </summary>
    public class Spaceship : MonoBehaviour, IInteractable
    {
        [Tooltip("The sound to play when the spaceship is being interacted with.")]
        [SerializeField]
        private AudioClip interactionSound;

        [Tooltip("The point where the player is spawned when they exit the ship.")]
        [SerializeField]
        private GameObject playerExitPoint;

        [SerializeField]
        [Tooltip("The dialogue host to use. Defaults to the host attached to the GameObject.")]
        private DialogueHost dialogueHost;

        [SerializeField]
        private YarnProgram deniedDialogue;

        /// <summary>
        /// Gets the point where the player is spawned when they exit the ship.
        /// </summary>
        public GameObject PlayerExitPoint => this.playerExitPoint;

        /// <inheritdoc/>
        public void OnInteract(GameObject obj)
        {
            Debug.Log("Interacted with Spaceship, healed Player");
            obj.GetComponent<PlayerEntity>()?.RestoreEnergy();
            AudioManager.Instance.PlayOneShot(this.interactionSound);

            if (QuestManager.Instance.HasCompletedQuest("SpaceshipPrerequisite"))
            {
                // Go to the next planet.
            }
            else
            {
                // Show the dialogue for the first time.
                this.StartDialogue(obj);
            }
        }

        /// <summary>
        /// Starts the associated dialogue.
        /// </summary>
        /// <param name="interacter">The entity interacting with the spaceship.</param>
        public void StartDialogue(GameObject interacter)
        {
            if (this.dialogueHost == null)
            {
                this.dialogueHost = this.GetComponent<DialogueHost>();
                if (this.dialogueHost == null)
                {
                    Debug.LogError($"{this.name} is missing a DialogueHost component.");
                    return;
                }
            }

            this.dialogueHost.StartDialogue(interacter, this.deniedDialogue.name);
        }
    }
}
