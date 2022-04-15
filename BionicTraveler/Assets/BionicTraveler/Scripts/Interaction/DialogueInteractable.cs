namespace BionicTraveler.Scripts.Interaction
{
    using UnityEngine;

    /// <summary>
    /// An object that, when interacted with, will prompt the player with some dialogue.
    /// </summary>
    public abstract class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        [Tooltip("The dialogue host to use. Defaults to the host attached to the GameObject.")]
        private DialogueHost dialogueHost;

        [SerializeField]
        private YarnProgram dialogue;

        /// <summary>
        /// Gets a value indicating whether or not the dialogue is finished running.
        /// </summary>
        public bool HasRun => this.dialogueHost.HasRun;

        /// <inheritdoc/>
        public virtual void OnInteract(GameObject obj)
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

            Debug.Log("Name is " + this.dialogue.name);
            this.dialogueHost.StartDialogue(obj, this.dialogue.name);
        }
    }
}
