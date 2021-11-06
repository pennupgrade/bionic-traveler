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
        
        // Should not be able to make a quest without name and description
        private Quest() { }

        // Factory method to create a quest GameObject with components
        public static GameObject NewQuest(string questName, string description, GameObject buttonPrefab)
        {
            GameObject quest = Instantiate(buttonPrefab);
            quest.name = "Quest: " + questName;
            quest.SetActive(true);
            
            Quest questQuest = quest.AddComponent<Quest>();
            questQuest.questName = questName;
            questQuest.description = description;
            questQuest.isCompleted = false;
            
            Button questButton = quest.GetComponent<Button>();
            questButton.name = quest.name;
            questButton.onClick.AddListener(questQuest.OnClick);

            Text text = questButton.GetComponentInChildren<Text>();
            text.text = questName;
            
            return quest;
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
            // Also call Quest Manager's function
        }

        // Quest's button in Quest Menu
        public void OnClick()
        {
            QuestManager.Instance.QuestClicked(name, description);
        }

        // Abstract method for listeners for quest completion
        public void Listener()
        {
            // Does nothing for now
        }
    }
}
