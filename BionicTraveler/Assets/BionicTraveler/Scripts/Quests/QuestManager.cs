namespace BionicTraveler.Scripts.Quests
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The main quest manager that keeps track of all quests.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        private static QuestManager instance;

        private List<Quest> currentQuests;
        private List<Quest> completedQuests;
        private Quest activeQuest;

        [SerializeField]
        private List<Quest> quests;

        private QuestUI ui;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestManager"/> class.
        /// </summary>
        public QuestManager()
        {
            this.currentQuests = new List<Quest>();
            this.completedQuests = new List<Quest>();
        }

        /// <summary>
        /// Delegate to inform about quest progress.
        /// </summary>
        /// <param name="quest">The quest</param>
        public delegate void QuestProgressChangedEventHandler(Quest quest);

        /// <summary>
        /// Event to inform about when a quest has been completed.
        /// </summary>
        public event QuestProgressChangedEventHandler OnQuestFinished;

        /// <summary>
        /// Gets the quest manager instance.
        /// </summary>
        public static QuestManager Instance => QuestManager.instance;

        private void Awake()
        {
            if (QuestManager.instance != null && QuestManager.instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                QuestManager.instance = this;
            }
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
            this.ui = GameObject.Find("PlayerQuestUI").GetComponent<QuestUI>();
            if (Debug.isDebugBuild)
            {
                foreach (Quest q in this.quests)
                {
                    this.AddQuest(q);
                }
            }
        }

        /// <summary>
        /// Checks if the quest is completed.
        /// </summary>
        /// <param name="name">The name of the quest.</param>
        /// <returns>Whether the quest has been completed.</returns>
        public bool HasCompletedQuest(string name)
            => this.completedQuests.Any(quest => quest.Title == name);

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

            // Spawn Notification Object
            this.ui.AddToCompleteList(quest);

            // Fire event.
            var questCompletedEvent = new QuestEventQuestCompleted(quest.Title);
            this.ProcessEvent(questCompletedEvent);
        }
    }
}
