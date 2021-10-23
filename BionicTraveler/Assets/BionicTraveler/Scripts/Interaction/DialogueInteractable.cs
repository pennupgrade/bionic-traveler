namespace BionicTraveler.Scripts.Interaction
{
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

        /// <inheritdoc/>
        public virtual void OnInteract(GameObject obj)
        {
            Debug.Log("NPC has been interacted by " + obj.name);
            this.runner.StartDialogue(this.dialogueStartNode);
        }
    }
}
