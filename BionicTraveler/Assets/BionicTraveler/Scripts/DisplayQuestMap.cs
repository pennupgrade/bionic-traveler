namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class DisplayQuestMap : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
        }

        public void launchImage()
        {
            //Debug.Log(Application.dataPath + @"BionicTraveler/Levels/Portfolio Construction/QuestMap.drawio.png");
            System.Diagnostics.Process.Start(Application.dataPath+@"/BionicTraveler/Levels/Portfolio Construction/QuestMap.drawio.png");
        }
    }
}
