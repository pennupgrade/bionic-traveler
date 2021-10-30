namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class ButtonListQuest : MonoBehaviour {

        [SerializeField]
        public Text quest;
        public Text questName;
        public Text questDescrip;
        public GameObject DescripBox;
        private List<string> questNames = new List<string>();
        private List<string> questDescrips = new List<string>();

        public void setText(string name)
        {
            quest.text = name;
        }

        public void setBox(string name, string des)
        {
            questName.text = name;
            questDescrip.text = des;
        }

        public void addBox(string name, string des)
        {
            questNames.Add(name);
            questDescrips.Add(des);
        }

        public void onClick()
        {
            if (DescripBox.activeInHierarchy == true)
            {
                DescripBox.SetActive(false);
            } else
            {
                DescripBox.SetActive(true);
            }
        }
    }
}
