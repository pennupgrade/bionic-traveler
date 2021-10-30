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

        public string strName;
        public string strDesc;
        public Text questName;
        public Text questDescrip;
        public GameObject DescripBox;
        //private static List<string> questNames = new List<string>();
        //private static List<string> questDescrips = new List<string>();
        private int i;

        public void setText(string name)
        {
            strName = name;
            quest.text = name;
        }

        public void setDesc(string Desc)
        {
            strDesc = Desc;
            questDescrip.text = Desc;
        }

        public void setBox(string name, string des)
        {
            questName.text = name;
            questDescrip.text = des;
        }
/*
        public static void addBox(string name, string des)
        {
            //questNames.Add(name);
            //questDescrips.Add(des);
        }
*/
        public void onClick()
        {
            if (DescripBox.activeInHierarchy == true)
            {
                DescripBox.SetActive(false);
            } else
            {
                
                DescripBox.SetActive(true);
                setBox(strName, strDesc);
            }
        }
    }
}
