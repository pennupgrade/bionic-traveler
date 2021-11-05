namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }
        
        private List<Quest> activeQuests = new List<Quest>();
        private List<Quest> completedQuests = new List<Quest>();

        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            } else {
                Instance = this;
            }
        }

        public void addQuest(string name, string description)
        {
            Quest quest = new Quest(name, description);
            questListNames.Add(name);
            questListDescrips.Add(des);
            questButtons.Add(Instantiate(buttonTemplate) as GameObject);
        }

        void Start()
        {
            addQuest("Acquire Metal Arm", "Go get that metal arm, girl. I don't really know how.");
            for (int i = 0; i < 20; i++)
            {
                addQuest("BLANK"+i, "Nothing");
                //ButtonListQuest.addBox("BLANK" + i, "Nothing");
            }

            for(int i = 0; i < questCount; i++)
            {
                GameObject button = questButtons[i];
                button.SetActive(true);
                questCompleted.Add(false);
                //GameObject textbox = Instantiate(boxTemplate) as GameObject;
                //textbox.SetActive(true);

                button.GetComponent<Quest>().setText(questListNames[i]);
                button.GetComponent<Quest>().setDesc(questListDescrips[i]);
                //button.GetComponent<ButtonListQuest>().setBox(questListNames[i], questListDescrips[i]);
                //textbox.GetComponent<ButtonListQuest>().setBox(questListNames[i], questListDescrips[i]);

                button.transform.SetParent(buttonTemplate.transform.parent, false);
                //textbox.transform.SetParent(boxTemplate.transform.parent, false);
            }
        }
        
        void Update()
        {
            for (int i = 0; i < questCount; i++)
            {
                if (questCompleted[i])
                {
                    questButtons[i].GetComponent<Quest>().setText("COMPLETED");
                } else
                {
                    questButtons[i].GetComponent<Quest>().setText(questListNames[i]);
                }
            }
        }
    }
}
