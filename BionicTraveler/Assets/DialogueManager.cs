namespace BionicTraveler
{
    using BionicTraveler.Scripts.Interaction;
    using UnityEngine;
    using UnityEngine.UI;
    using Yarn.Unity;

    /// <summary>
    /// Provides supporting functions for in-game dialogues such as setting up character name and avatars
    /// based on the associated dialogue.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text characterName;
        [SerializeField]
        private Image characterImage;

        private DialogueRunner runner;
        private DialogueUI ui;
        private DialogueInteractable currentSource;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.runner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();

            this.ui = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueUI>();
            this.ui.onDialogueStart.AddListener(this.DialogueStartListener);
            this.ui.onLineStart.AddListener(this.DialogueStartListener);
        }

        /// <summary>
        /// Starts a new dialogue at the specified start node.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dialogue">The dialogue.</param>
        /// <param name="startNode">The start node.</param>
        public void StartNewDialogue(DialogueInteractable source, YarnProgram dialogue, string startNode)
        {
            this.currentSource = source;
            this.runner.Add(dialogue);
            this.runner.StartDialogue(startNode);
        }

        private void DialogueStartListener()
        {
            var avatarName = this.runner.variableStorage.GetValue("$npcface");
            if (avatarName.type == Yarn.Value.Type.Null)
            {
                this.characterName.text = string.IsNullOrWhiteSpace(this.currentSource.OverrideCharacterName)
                    ? "Anonymous" : this.currentSource.OverrideCharacterName;
            }
            else
            {
                this.characterName.text = avatarName.AsString;
            }

            //Debug.Log("Dialogue set \"$npcface\" to " + avatarName.AsString);
        }

        private void OnDestroy()
        {
            this.ui.onDialogueStart.RemoveListener(this.DialogueStartListener);
            this.ui.onLineStart.RemoveListener(this.DialogueStartListener);
        }
    }
}
