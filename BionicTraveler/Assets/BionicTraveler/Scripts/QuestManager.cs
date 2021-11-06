using UnityEngine.UI;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {
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
        
        private List<GameObject> activeQuests = new List<GameObject>();
        private List<GameObject> completedQuests = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            } else {
                Instance = this;
            }
        }

        public void AddQuest(string questName, string description)
        {
            GameObject quest = Quest.NewQuest(questName, description, buttonPrefab);
            activeQuests.Add(quest);
            quest.transform.SetParent(questContainer.transform);
        }

        public void CompleteQuest(string questName)
        {
            // Possibly more efficient way of doing this?
            foreach (GameObject quest in activeQuests)
            {
                Quest script = quest.GetComponent<Quest>();
                if (script.GetQuestName() == questName)
                {
                    activeQuests.Remove(quest);
                    completedQuests.Add(quest);
                    quest.transform.SetParent(null);
                    quest.transform.SetParent(questContainer.transform);
                    Text text = quest.GetComponentInChildren<Text>();
                    text.text = "COMPLETED";
                    return;
                }
            }
        }

        public void QuestClicked(string questName, string description)
        {
            if (questOpened == null)
            {
                questDetails.SetActive(true);
                questOpened = questName;
            } else if (questOpened == questName)
            {
                questDetails.SetActive(false);
                questOpened = null;
            }
            else
            {
                questOpened = questName;
            }

            questTitle.text = questName;
            questDescription.text = description;
        }

        public void Start()
        {
            // Search can be improved, just attempting to do search via tree rather than drag and drop
            GameObject self = this.gameObject;
            for (int i = 0; i < 20; i++)
            {
                AddQuest("temp" + i, i.ToString());
            }

            // AddQuest("temp1", "first one");
        }


    }
}
