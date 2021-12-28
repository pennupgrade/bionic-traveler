namespace BionicTraveler
{
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

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.runner = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueRunner>();

            this.ui = GameObject.FindGameObjectWithTag("DialogueRunner").GetComponent<DialogueUI>();
            this.ui.onLineStart.AddListener(this.LineStartListener);
        }

        private void LineStartListener()
        {
            var avatarName = this.runner.variableStorage.GetValue("$npcface");
            this.characterName.text = avatarName.AsString;

            //Debug.Log("Dialogue set \"$npcface\" to " + avatarName.AsString);
        }
    }
}
