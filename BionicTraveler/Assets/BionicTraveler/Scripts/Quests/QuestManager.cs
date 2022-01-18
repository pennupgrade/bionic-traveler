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
        private List<Quest> currentQuests;
        private List<Quest> completedQuests;
        private Quest activeQuest;

        [SerializeField]
        private Quest debugQuest;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestManager"/> class.
        /// </summary>
        public QuestManager()
        {
            this.currentQuests = new List<Quest>();
            this.completedQuests = new List<Quest>();
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
            if (Debug.isDebugBuild)
            {
                this.AddQuest(this.debugQuest);
            }
        }

        private void AddQuest(Quest quest)
        {
            quest.Initialize(this);
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
            this.activeQuest.StateChanged += this.ActiveQuest_StateChanged;
            this.activeQuest.SetAsActiveQuest(true);
            Debug.Log("New active quest set!");
        }

        private void ActiveQuest_StateChanged(Quest sender, QuestState newState)
        {
            Debug.Log("Our active quest changed its state to " + newState);
            if (newState == QuestState.Finished)
            {
                this.ArchiveActiveQuest(sender);
            }
        }

        private void ArchiveActiveQuest(Quest quest)
        {
            Debug.Log("Action quest has finished, archiving");
            this.currentQuests.Remove(quest);
            this.completedQuests.Add(quest);
            this.activeQuest = null;
        }
    }
}
