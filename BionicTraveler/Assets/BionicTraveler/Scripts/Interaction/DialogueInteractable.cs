namespace BionicTraveler.Scripts.Interaction
{
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using Yarn.Unity;

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

        [SerializeField]
        [Tooltip("Overrides the name of the dialogue's character, if any.")]
        private string overrideCharacterName;

        [SerializeField]
        [Tooltip("Disables speech audio.")]
        private bool disableSpeechAudio;

        private DialogueRunner runner;
        private bool hasRun;

        private DialogueUI ui;

        /// <summary>
        /// Gets the associated yarn dialogue.
        /// </summary>
        public YarnProgram Dialogue => this.dialogue;

        /// <summary>
        /// Gets the override character name, that is the name used instead of the dialogue's configured
        /// character name.
        /// </summary>
        public string OverrideCharacterName => this.overrideCharacterName;

        /// <summary>
        /// Gets a value indicating whether or not the dialogue is finished running.
        /// </summary>
        public bool HasRun => this.hasRun;

        /// <inheritdoc/>
        public virtual void OnInteract(GameObject obj)
        {
            if (this.hasRun && !this.multipleRuns)
            {
                Debug.Log("Dialogue is not multirun and already ran. Cannot run again!");
                return;
            }

            Debug.Log("NPC has been interacted by " + obj.name);
            var questManager = GameObject.FindObjectOfType<Quests.QuestManager>();

            var speakWithQuest = new Quests.QuestEventSpokenTo(this.overrideCharacterName);
            questManager.ProcessEvent(speakWithQuest);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = true;
            player.IsInDialogue = true;
            player.DisableInput();

            var runnerGameobject = GameObject.FindGameObjectWithTag("DialogueRunner");
            this.ui = runnerGameobject.GetComponent<DialogueUI>();
            this.ui.onLineStart.AddListener(this.LineStartListener);
            this.ui.onLineFinishDisplaying.AddListener(this.LineFinishDisplayingListener);

            var manager = runnerGameobject.GetComponent<DialogueManager>();
            manager.StartNewDialogue(this, this.dialogue, this.dialogueStartNode);

            this.runner = runnerGameobject.GetComponent<DialogueRunner>();
            this.runner.onDialogueComplete.AddListener(this.DialogueCompletedHandler);
        }

        private void LineStartListener()
        {
            if (!this.disableSpeechAudio)
            {
                AudioManager.Instance.PlayDialogueSound();
            }
        }

        private void LineFinishDisplayingListener()
        {
            if (!this.disableSpeechAudio)
            {
                AudioManager.Instance.StopDialogueSound();
            }
        }

        private void DialogueCompletedHandler()
        {
            // We need to clear the runner, otherwise we cannot load the same program again
            // or other programs with nodes with the same names.
            this.hasRun = true;
            this.runner.Clear();
            this.runner.onDialogueComplete.RemoveListener(this.DialogueCompletedHandler);
            this.ui.onLineStart.RemoveListener(this.LineStartListener);
            this.ui.onLineFinishDisplaying.RemoveListener(this.LineFinishDisplayingListener);
            DialogueStates.Instance.MarkDialogueAsCompleted(this, this.persistState);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = false;
            player.IsInDialogue = false;
            player.EnableInput();
        }
    }
}
