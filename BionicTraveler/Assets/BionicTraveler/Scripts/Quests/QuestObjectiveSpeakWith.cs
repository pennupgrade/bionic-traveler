namespace BionicTraveler.Scripts.Quests
{
    using UnityEngine;

    /// <summary>
    /// Quest objective for speaking with a NPC.
    /// </summary>
    [System.Serializable]
    public class QuestObjectiveSpeakWith : QuestObjective
    {
        [SerializeField]
        private string targetEntityName;

        /// <inheritdoc/>
        public override void Activate()
        {
            base.Activate();
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            var objective = new QuestObjectiveSpeakWith();
            objective.CloneBase(objective);
            objective.targetEntityName = this.targetEntityName;
            return objective;
        }

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEventSpokenTo">The event.</param>
        public void ProcessEvent(QuestEventSpokenTo questEventSpokenTo)
        {
            this.CheckTargetName(questEventSpokenTo.NpcName);
        }

        private void CheckTargetName(string name)
        {
            if (name == this.targetEntityName)
            {
                Debug.Log("Complete Quest: Spoken to " + name + "!");
                this.SetAsComplete();
            }
        }
    }
}