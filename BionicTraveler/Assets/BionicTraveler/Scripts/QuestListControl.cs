namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class QuestListControl : MonoBehaviour
    {
        [SerializeField]
        public GameObject buttonTemplate;
        public bool questCompleted;
        //public GameObject boxTemplate;
        public List<GameObject> questButtons = new List<GameObject>();
        private int questCount = 0;
        private List<string> questListNames = new List<string>();
        private List<string> questListDescrips = new List<string>();

        void addQuest(string name, string des)
        {
            questCount += 1;
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
                //GameObject textbox = Instantiate(boxTemplate) as GameObject;
                //textbox.SetActive(true);

                button.GetComponent<ButtonListQuest>().setText(questListNames[i]);
                button.GetComponent<ButtonListQuest>().setDesc(questListDescrips[i]);
                //button.GetComponent<ButtonListQuest>().setBox(questListNames[i], questListDescrips[i]);
                //textbox.GetComponent<ButtonListQuest>().setBox(questListNames[i], questListDescrips[i]);

                button.transform.SetParent(buttonTemplate.transform.parent, false);
                //textbox.transform.SetParent(boxTemplate.transform.parent, false);
            }
        }

        void Update()
        {
            if (questCompleted)
            {
                questButtons[0].GetComponent<ButtonListQuest>().setText("COMPLETED");
            }
        }
    }
}
