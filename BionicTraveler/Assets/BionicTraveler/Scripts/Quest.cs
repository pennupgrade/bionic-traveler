namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    // May eventually make this an abstract class and have specific quests implement this
    // class for different listeners

    /// <summary>
    /// Quest class that stores quest name, description, and completion status.
    /// </summary>
    public class Quest : MonoBehaviour
    {
        private string questName;
        private string description;
        private bool isCompleted;

        private int clickedCount = 0;

        // Should not be able to make a quest without name and description
        private Quest()
        {
        }

        /// <summary>
        /// Factory method to create a quest GameObject with components.
        /// </summary>
        /// <param name="questName">the name of the quest.</param>
        /// <param name="description">the description of the quest.</param>
        /// <param name="buttonPrefab">the button template that is copied to make new quest buttons.</param>
        /// <returns> GameObject button.</returns>
        public static GameObject NewQuest(string questName, string description, GameObject buttonPrefab)
        {
            GameObject quest = Instantiate(buttonPrefab);
            quest.name = "Quest: " + questName;
            quest.SetActive(true);

            // makes a Quest object and sets name, description, and completion status
            Quest questQuest = quest.AddComponent<Quest>();
            questQuest.questName = questName;
            questQuest.description = description;
            questQuest.isCompleted = false;

            // makes a button, adds a listener that tracks if button has been clicked
            Button questButton = quest.GetComponent<Button>();
            questButton.name = quest.name;
            questButton.onClick.AddListener(questQuest.OnClick);

            // sets the text displayed on the button
            Text text = questButton.GetComponentInChildren<Text>();
            text.text = questName;

            return quest;
        }

        /// <summary>
        /// gets name of quest.
        /// </summary>
        /// <returns>string that is the name of quest.</returns>
        public string GetQuestName()
        {
            return this.questName;
        }

        /// <summary>
        /// sets name of quest.
        /// </summary>
        /// <param name="newName">name that should be set as quest name.</param>
        public void SetName(string newName)
        {
            this.name = newName;
        }

        /// <summary>
        /// sets the description of quest.
        /// </summary>
        /// <param name="newDescription">description that should be set as quest's description.</param>
        public void SetDescription(string newDescription)
        {
            this.description = newDescription;
        }

        /// <summary>
        /// sets the status of quest completion to true if quest is completed and false if not completed.
        /// </summary>
        /// <param name="completed">boolean that is true if quest is completed and false otherwise.</param>
        public void SetIsCompleted(bool completed)
        {
            this.isCompleted = completed;
            QuestManager.Instance.CompleteQuest(this.questName);
        }

        /// <summary>
        /// this function is called if button is clicked
        /// calls an instance of QuestManager that opens the description box
        /// increments clickedCount
        /// if the number of times you click a button = 5, the quest is completed.
        /// </summary>
        public void OnClick()
        {
            QuestManager.Instance.QuestClicked(this.name, this.description);
            this.clickedCount += 1;
            if (this.clickedCount == 5)
            {
                this.SetIsCompleted(true);
            }
        }

        /// <summary>
        /// Abstract method for listeners for quest completion.
        /// </summary>
        public void Listener()
        {
        }
    }
}
