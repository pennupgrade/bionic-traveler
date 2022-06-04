namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using TMPro;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class HandleTxt : MonoBehaviour
    {
        private TextMeshProUGUI fanficText;
        private String readInStream;
        private StreamReader reader;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            fanficText = gameObject.GetComponent<TextMeshProUGUI>();

            string path = "Assets/BionicTraveler/Levels/Portfolio Construction/David's Garbage Fanfic.txt";
            reader = new StreamReader(path);
            pageTurn();
            
        }

        void pageTurn()
        {
            fanficText.SetText(reader.ReadToEnd());
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            //fanficText.SetText("Work damnit");
        }
    }
}
