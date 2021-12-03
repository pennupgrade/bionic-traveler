namespace BionicTraveler.Scripts.Interaction
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using Yarn.Unity;

    /// <summary>
    /// An object that, when interacted with, will prompt the player with some dialogue.
    /// </summary>
    public abstract class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DialogueRunner runner;

        [SerializeField]
        private string dialogueStartNode;

        [SerializeField]
        [Tooltip("Whether dialogue can be executed multiple times.")]
        private bool multipleRuns;

        [SerializeField]
        [Tooltip("Whether dialogue state will be saved in save file.")]
        private bool persistState;

        private bool hasRun;

        /// <inheritdoc/>
        public virtual void OnInteract(GameObject obj)
        {
            if (this.hasRun && !this.multipleRuns)
            {
                Debug.Log("Dialogue is not multirun and already ran. Cannot run again!");
                return;
            }

            Debug.Log("NPC has been interacted by " + obj.name);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = true;
            player.DisableInput();

            this.runner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();
            this.runner.StartDialogue(this.dialogueStartNode);

            this.runner.onDialogueComplete.AddListener(() =>
            {
                this.hasRun = true;

                player.IsIgnoredByEveryone = false;
                player.EnableInput();

                Debug.Log("Has Run state: " + this.hasRun);
            });
        }
    }
}
