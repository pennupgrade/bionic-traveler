namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The main quest manager that keeps track of all quests.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public delegate void QuestFinishedEventHandler(Quest quest);

        public event QuestFinishedEventHandler OnQuestFinished;

        private List<Quest> currentQuests;
        private List<Quest> completedQuests;
        private Quest activeQuest;

        [SerializeField]
        private List<Quest> quests;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestManager"/> class.
        /// </summary>
        public QuestManager()
        {
            this.currentQuests = new List<Quest>();
            this.completedQuests = new List<Quest>();
        }

        public List<Quest> getCompletedQuests()
        {
            return this.completedQuests;
        }

        public List<Quest> getCurrentQuests()
        {
            return this.currentQuests;
        }

        /// <summary>
        /// Processes a new quest event.
        /// </summary>
        /// <param name="questEvent">The event.</param>
        public void ProcessEvent(QuestEvent questEvent)
        {
            // Events might lead to changes in our enumeration so we copy it.
            foreach (var quest in this.currentQuests.ToArray())
            {
                quest.ProcessEvent(questEvent);
            }
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            Debug.Log("Quests Have started");
            if (Debug.isDebugBuild)
            {
                foreach (Quest q in this.quests)
                {
                    this.AddQuest(q);
                }
            }
        }

        private void AddQuest(Quest quest)
        {
            quest.Initialize(this);
            quest.StateChanged += this.Quest_StateChanged;
            this.currentQuests.Add(quest);

            // First quest, active by default.
            if (this.currentQuests.Count == 1)
            {
                this.SetActiveQuest(this.currentQuests[0]);
            }
        }

        private void SetActiveQuest(Quest quest)
        {
            if (this.activeQuest != null)
            {
                this.activeQuest.SetAsActiveQuest(false);
            }

            this.activeQuest = quest;
            this.activeQuest.SetAsActiveQuest(true);
            Debug.Log("New active quest set!");
        }

        private void Quest_StateChanged(Quest sender, QuestState newState)
        {
            Debug.Log("Our active quest changed its state to " + newState);
            if (newState == QuestState.Finished)
            {
                this.OnQuestFinished?.Invoke(sender);
                this.ArchiveQuest(sender);
            }
        }

        private void ArchiveQuest(Quest quest)
        {
            Debug.Log("Quest has finished, archiving");
            quest.StateChanged -= this.Quest_StateChanged;
            this.currentQuests.Remove(quest);
            this.completedQuests.Add(quest);

            if (this.activeQuest == quest)
            {
                this.activeQuest = null;
            }
        }
    }
}
