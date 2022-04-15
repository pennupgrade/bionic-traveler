namespace BionicTraveler.Scripts.Interaction
{
    using System;
        using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Quests;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using Yarn.Unity;

    /// <summary>
    /// Hosts a dialogue with several options.
    /// </summary>
    [Serializable]
    public class DialogueData
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

        /// <summary>
        /// Gets the associated yarn dialogue.
        /// </summary>
        public YarnProgram Dialogue => this.dialogue;

        /// <summary>
        /// Gets the name of the underlying dialogue.
        /// </summary>
        public string Name => this.dialogue.name;

        /// <summary>
        /// Gets the override character name, that is the name used instead of the dialogue's configured
        /// character name.
        /// </summary>
        public string OverrideCharacterName => this.overrideCharacterName;

        /// <summary>
        /// Gets a value indicating whether dialogue can be executed multiple times.
        /// </summary>
        public bool AllowMultipleRuns => this.multipleRuns;

        /// <summary>
        /// Gets a value indicating whether speech audio should be disabled.
        /// </summary>
        public bool DisableSpeechAudio => this.disableSpeechAudio;

        /// <summary>
        /// Gets the dialogue start node.
        /// </summary>
        public string DialogueStartNode => this.dialogueStartNode;

        /// <summary>
        /// Gets a value indicating whether dialogue state should be saved in save file.
        /// </summary>
        public bool PersistState => this.persistState;
    }
}
