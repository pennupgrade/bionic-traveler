namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        private List<Quest> currentQuests;
        private List<Quest> completedQuests;
        private Quest activeQuest;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestManager"/> class.
        /// </summary>
        public QuestManager()
        {
            this.currentQuests = new List<Quest>();
            this.completedQuests = new List<Quest>();
        }

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
    }
}
