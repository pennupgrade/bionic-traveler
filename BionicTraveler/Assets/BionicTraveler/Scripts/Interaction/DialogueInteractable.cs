namespace BionicTraveler.Scripts.Interaction
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using Yarn.Unity;
    using BionicTraveler.Scripts.Audio;

    /// <summary>
    /// An object that, when interacted with, will prompt the player with some dialogue.
    /// </summary>
    public abstract class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private YarnProgram dialogue;

        [SerializeField]
        [DialogueNodeSelectorAttribute(nameof(dialogue))]
        private string dialogueStartNode;

        [SerializeField]
        [Tooltip("Whether dialogue can be executed multiple times.")]
        private bool multipleRuns;

        [SerializeField]
        [Tooltip("Whether dialogue state will be saved in save file.")]
        private bool persistState;

        private DialogueRunner runner;
        private bool hasRun;

        private DialogueUI ui;

        /// <summary>
        /// Gets the associated yarn dialogue.
        /// </summary>
        public YarnProgram Dialogue => this.dialogue;

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
            
            this.ui = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueUI>();
            this.ui.onLineStart.AddListener(this.LineStartListener);
            this.ui.onLineFinishDisplaying.AddListener(this.LineFinishDisplayingListener);

            this.runner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();
            this.runner.Add(this.dialogue);
            this.runner.StartDialogue(this.dialogueStartNode);
            this.runner.onDialogueComplete.AddListener(this.DialogueCompletedHandler);

            
            

        }

        private void LineStartListener() {
            AudioManager.Instance.PlayDialogueSound();
        }

        private void LineFinishDisplayingListener() {
            AudioManager.Instance.StopDialogueSound();
        }

        private void DialogueCompletedHandler()
        {
            // We need to clear the runner, otherwise we cannot load the same program again
            // or other programs with nodes with the same names.
            this.hasRun = true;
            this.runner.Clear();
            this.runner.onDialogueComplete.RemoveListener(this.DialogueCompletedHandler);
            DialogueStates.Instance.MarkDialogueAsCompleted(this, this.persistState);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = false;
            player.EnableInput();
        }
    }
}
