namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// class that manages all the quests
    /// stores a container for all the quests, the description box of the quest, the title of the description box,
    /// and the button template that gets copied everytime you create a quest.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        /// <summary>
        /// Gets an instance of the QuestManager class -> used to make sure that there is the only one instance of QuestManager.
        /// </summary>
        public static QuestManager Instance { get; private set; }

        [SerializeField]
        private GameObject questContainer;
        [SerializeField]
        private GameObject questDetails;
        [SerializeField]
        private Text questTitle;
        [SerializeField]
        private Text questDescription;
        [SerializeField]
        private GameObject buttonPrefab;

        private string questOpened = null;

        private List<GameObject> activeQuests = new ();
        private List<GameObject> completedQuests = new ();

        /// <summary>
        /// Adds a quest using inputted questName and description
        /// creates a GameObject by calling the NewQuest method in Quest
        /// adds quest to activeQuests list
        /// sets the parent of the GameObject to questContainer.
        /// </summary>
        /// <param name="questName">string that is set to name of quest.</param>
        /// <param name="description">string that is set to description of quest.</param>
        public void AddQuest(string questName, string description)
        {
            GameObject quest = Quest.NewQuest(questName, description, this.buttonPrefab);
            this.activeQuests.Add(quest);
            quest.transform.SetParent(this.questContainer.transform);
        }

        /// <summary>
        /// Sets the quest to completed.
        /// </summary>
        /// <param name="questName">quest's name.</param>
        public void CompleteQuest(string questName)
        {
            // Possibly more efficient way of doing this?
            foreach (GameObject quest in this.activeQuests)
            {
                Quest script = quest.GetComponent<Quest>();
                if (script.GetQuestName() == questName)
                {
                    this.activeQuests.Remove(quest);
                    this.completedQuests.Add(quest);
                    quest.transform.SetParent(null);
                    quest.transform.SetParent(this.questContainer.transform);
                    Text text = quest.GetComponentInChildren<Text>();
                    text.text = "COMPLETED";
                    return;
                }
            }
        }

        /// <summary>
        /// function is called when a quest's button is clicked
        /// displays the description box of the quest on first click
        /// hides the description box of the quest on thesecond click.
        /// </summary>
        /// <param name="questName">quest's name.</param>
        /// <param name="description">quest's description.</param>
        public void QuestClicked(string questName, string description)
        {
            if (this.questOpened == null)
            {
                this.questDetails.SetActive(true);
                this.questOpened = questName;
            } else if (this.questOpened == questName)
            {
                this.questDetails.SetActive(false);
                this.questOpened = null;
            }
            else
            {
                this.questOpened = questName;
            }

            this.questTitle.text = questName;
            this.questDescription.text = description;
        }

        /// <summary>
        /// Called when the program is first started
        /// Currently loops through and creates 20 quests.
        /// </summary>
        public void Start()
        {
            // Search can be improved, just attempting to do search via tree rather than drag and drop
            for (int i = 0; i < 20; i++)
            {
                this.AddQuest("temp" + i, i.ToString());
            }

            // AddQuest("temp1", "first one");
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
