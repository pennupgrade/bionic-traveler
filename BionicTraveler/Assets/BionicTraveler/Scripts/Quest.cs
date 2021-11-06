namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    // May eventually make this an abstract class and have specific quests implement this
    // class for different listeners
    
    public class Quest : MonoBehaviour {
        
        private string questName;
        private string description;
        private bool isCompleted;

        private int clickedCount = 0;
        
        // Should not be able to make a quest without name and description
        private Quest() { }

        // Factory method to create a quest GameObject with components
        public static GameObject NewQuest(string questName, string description, GameObject buttonPrefab)
        {
            GameObject quest = Instantiate(buttonPrefab);
            quest.name = "Quest: " + questName;
            quest.SetActive(true);
            
            //makes a Quest object and sets name, description, and completion status
            Quest questQuest = quest.AddComponent<Quest>();
            questQuest.questName = questName;
            questQuest.description = description;
            questQuest.isCompleted = false;
            
            //makes a button, adds a listener that tracks if button has been clicked
            Button questButton = quest.GetComponent<Button>();
            questButton.name = quest.name;
            questButton.onClick.AddListener(questQuest.OnClick);

            //sets the text desplayed on the button
            Text text = questButton.GetComponentInChildren<Text>();
            text.text = questName;
            
            return quest;
        }

        public string GetQuestName()
        {
            return questName;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public void SetDescription(string newDescription)
        {
            description = newDescription;
        }

        public void SetIsCompleted(bool completed)
        {
            isCompleted = completed;
            QuestManager.Instance.CompleteQuest(questName);
        }

        // Quest's button in Quest Menu
        public void OnClick()
        {
            QuestManager.Instance.QuestClicked(name, description);
            clickedCount += 1;
            if (clickedCount == 5) {
                SetIsCompleted(true);
            }
        }

        // Abstract method for listeners for quest completion
        public void Listener()
        {
        }
    }
}
