namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    // May eventually make this an abstract class and have specific quests implement this
    // class for different listeners
    
    public class Quest {
        
        private string name;
        private string description;
        private bool isCompleted;

        private QuestManager manager = QuestManager.Instance;

        // Should not be able to make a quest without name and description
        private Quest() { }

        public Quest(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.isCompleted = false;
        }

        public void setName(string newName)
        {
            name = newName;
        }

        public void setDescription(string newDescription)
        {
            description = newDescription;
        }

        public void setIsCompleted(bool completed)
        {
            isCompleted = completed;
            // Also call Quest Manager's function
        }

        // Quest's button in Quest Menu
        public void onClick()
        {
            if (DescripBox.activeInHierarchy == true)
            {
                //DescripBox.SetActive(false);
                setBox(strName, strDesc);
            } else
            {
                
                DescripBox.SetActive(true);
                setBox(strName, strDesc);
            }
 
        }

        // Abstract method for listeners for quest completion
        public void listener()
        {
        }
    }
}
