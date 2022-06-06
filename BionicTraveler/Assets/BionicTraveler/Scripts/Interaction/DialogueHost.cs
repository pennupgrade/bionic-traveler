namespace BionicTraveler.Scripts.Interaction
{
    using System;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Quests;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using Yarn.Unity;

    /// <summary>
    /// Behaviour that stores a list of dialogues that can be configured. Can run these dialogues.
    /// </summary>
    public class DialogueHost : MonoBehaviour
    {
        [SerializeField]
        private List<DialogueData> dialogues = new List<DialogueData>();

        private DialogueData dialogueData;
        private DialogueRunner runner;
        private bool hasRun;

        private DialogueUI ui;

        /// <summary>
        /// Event invoked when the dialogue has completed.
        /// </summary>
        public event Action<DialogueHost> DialogueCompleted;

        /// <summary>
        /// Gets a value indicating whether or not the dialogue is finished running.
        /// </summary>
        public bool HasRun => this.hasRun;

        /// <summary>
        /// Gets the name last used for a character by this host.
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the associated variable storage. This is only valid once a dialogue has been started.
        /// </summary>
        public VariableStorageBehaviour VariableStorage => this.runner.variableStorage;

        /// <summary>
        /// Starts the dialogue.
        /// </summary>
        /// <param name="interactor">The game object interacting with this dialogue host.</param>
        /// <param name="dialogue">The dialogue.</param>
        public void StartDialogue(GameObject interactor, string dialogue)
        {
            this.StartDialogue(interactor, dialogue, null, null);
        }

        /// <summary>
        /// Starts the dialogue.
        /// </summary>
        /// <param name="interactor">The game object interacting with this dialogue host.</param>
        /// <param name="dialogue">The dialogue.</param>
        /// <param name="characterName">The character name.</param>
        /// <param name="startNode">The startNode.</param>
        public void StartDialogue(GameObject interactor, string dialogue, string characterName, string startNode)
        {
            this.dialogueData = this.dialogues.Find(x => x.Name == dialogue);
            if (this.dialogueData == null)
            {
                Debug.LogError($"Dialogue with name {dialogue} not found.");
                return;
            }

            if (this.hasRun && !this.dialogueData.AllowMultipleRuns)
            {
                Debug.Log("Dialogue is not multirun and already ran. Cannot run again!");
                return;
            }

            characterName ??= this.dialogueData.OverrideCharacterName;
            startNode ??= this.dialogueData.DialogueStartNode;
            this.LastName = characterName;

            // MOVED TO COMPLETE DIALOGUE
            //Debug.Log("Dialogue has been started by " + interactor.name);
            //var speakWithQuest = new Quests.QuestEventSpokenTo(characterName);
            //QuestManager.Instance.ProcessEvent(speakWithQuest);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = true;
            player.IsInDialogue = true;
            player.DisableInput();

            var runnerGameobject = GameObject.FindGameObjectWithTag("DialogueRunner");
            this.ui = runnerGameobject.GetComponent<DialogueUI>();
            this.ui.onLineStart.AddListener(this.LineStartListener);
            this.ui.onLineFinishDisplaying.AddListener(this.LineFinishDisplayingListener);

            var manager = runnerGameobject.GetComponent<DialogueManager>();
            manager.StartNewDialogue(this.dialogueData, this.dialogueData.Dialogue, startNode, characterName);

            this.runner = runnerGameobject.GetComponent<DialogueRunner>();
            this.runner.onDialogueComplete.AddListener(this.DialogueCompletedHandler);
        }

        private void LineStartListener()
        {
            if (!this.dialogueData.DisableSpeechAudio)
            {
                AudioManager.Instance.PlayDialogueSound();
            }
        }

        private void LineFinishDisplayingListener()
        {
            if (!this.dialogueData.DisableSpeechAudio)
            {
                AudioManager.Instance.StopDialogueSound();
            }
        }

        private void DialogueCompletedHandler()
        {
            // We need to clear the runner, otherwise we cannot load the same program again
            // or other programs with nodes with the same names.
            this.hasRun = true;

            // Fire complete dialogue event
            Debug.Log("Dialogue has been started by " + this.LastName);
            var speakWithQuest = new Quests.QuestEventSpokenTo(this.LastName);
            QuestManager.Instance.ProcessEvent(speakWithQuest);

            this.runner.Clear();
            this.runner.onDialogueComplete.RemoveListener(this.DialogueCompletedHandler);
            this.ui.onLineStart.RemoveListener(this.LineStartListener);
            this.ui.onLineFinishDisplaying.RemoveListener(this.LineFinishDisplayingListener);
            DialogueStates.Instance.MarkDialogueAsCompleted(this.dialogueData, this.dialogueData.PersistState);

            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.IsIgnoredByEveryone = false;
            player.IsInDialogue = false;
            player.EnableInput();

            this.DialogueCompleted?.Invoke(this);
        }
    }
}