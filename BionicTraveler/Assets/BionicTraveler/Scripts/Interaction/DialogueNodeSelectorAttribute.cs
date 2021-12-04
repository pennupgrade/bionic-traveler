namespace BionicTraveler.Scripts.Interaction
{
    using UnityEngine;

    /// <summary>
    /// Attribute to designate a property to show dialogue node names.
    /// </summary>
    public class DialogueNodeSelectorAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueNodeSelectorAttribute"/> class.
        /// </summary>
        /// <param name="dialogue">The name of the dialogue variable.</param>
        public DialogueNodeSelectorAttribute(string dialogue)
        {
            this.Dialogue = dialogue;
        }

        /// <summary>
        /// Gets the name of the dialogue variable.
        /// </summary>
        public string Dialogue { get; }
    }
}
