namespace BionicTraveler.Scripts.Quests
{
    using UnityEngine;

    /// <summary>
    /// Quest objective for completing another quest.
    /// </summary>
    [System.Serializable]
    internal class QuestObjectiveCompleteQuest : QuestObjective
    {
        [SerializeField]
        private string questName;

        /// <inheritdoc/>
        public override void Activate()
        {
            base.Activate();
            this.CheckQuestCompletion();
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            var objective = new QuestObjectiveCompleteQuest();
            objective.CloneBase(objective);
            objective.questName = this.questName;
            return objective;
        }

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEventQuestCompleted">The event.</param>
        public void ProcessEvent(QuestEventQuestCompleted questEventQuestCompleted)
        {
            this.CheckQuestCompletion(questEventQuestCompleted.QuestName);
        }

        private void CheckQuestCompletion(string questCompleted = "")
        {
            if (!string.IsNullOrWhiteSpace(questCompleted))
            {
                if (questCompleted == this.questName)
                {
                    this.SetAsComplete();
                }
            }
            else
            {
                if (QuestManager.Instance.HasCompletedQuest(this.questName))
                {
                    this.SetAsComplete();
                }
            }

        }
    }
}