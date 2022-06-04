namespace BionicTraveler.Scripts.Interaction
{
    using System.Collections.Generic;
    using BionicTraveler.Assets.Framework;

    /// <summary>
    /// Stores the state of game dialogues, i.e. whether have been completed or not.
    /// Should become part of the save state in the future.
    /// TODO: Scriptable object? Investigate.
    /// </summary>
    public class DialogueStates : Singleton<DialogueStates>
    {
        private List<string> completedDialogues;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogueStates"/> class.
        /// </summary>
        public DialogueStates()
        {
            this.completedDialogues = new List<string>();
        }

        /// <summary>
        /// Marks the dialogue as completed so its state can be queried by other subsystems.
        /// </summary>
        /// <param name="dialogueInteractable">The dialogue.</param>
        /// <param name="persistent">Whether this state should be saved across game sessions.</param>
        public void MarkDialogueAsCompleted(DialogueData dialogueInteractable, bool persistent)
        {
            // TODO: Persist if necessary.
            this.completedDialogues.Add(dialogueInteractable.Name);
        }

        /// <summary>
        /// Adds a custom completion state by name.
        /// </summary>
        /// <param name="name">The name of the completion state.</param>
        public void SetCustomState(string name)
        {
            this.completedDialogues.Add(name);
        }

        /// <summary>
        /// Checks the given conditions against the global dialogue state.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <returns>Whether or not the conditions are evaluated to true.</returns>
        public bool CheckConditions(List<string> conditions)
        {
            if (conditions.Count == 0)
            {
                return true;
            }

            // TODO: Allow various operators on dialogue conditions.
            // For now, use logical OR.
            foreach (var condition in conditions)
            {
                if (this.completedDialogues.Contains(condition))
                {
                    return true;
                }
            }

            return false;
        }
    }
}