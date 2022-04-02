namespace BionicTraveler.Scripts.Quests
{
    using BionicTraveler.Assets.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        public GameObject notifPrefab;

        public GameObject currentNotif;

        private List<Quest> listAssignQuests;
        private List<Quest> listCompleteQuests;

        [SerializeField]
        private QuestManager questManager;

        private GameTime timeElapsedSinceBannerCreation;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.questManager = GameObject.Find("PlayerQuestManager").GetComponent<QuestManager>();
            this.listAssignQuests = new List<Quest>();
            this.listCompleteQuests = new List<Quest>();
        }

        public void InvokeAssignUI(Quest q)
        {
            // Only spawn a new notification if current notification is null
            if (!currentNotif)
            {
                this.currentNotif = GameObject.Instantiate(this.notifPrefab, this.questManager.transform.position, Quaternion.identity);

                this.currentNotif.transform.parent = this.transform;
                this.timeElapsedSinceBannerCreation = GameTime.Now;
            }
        }

        public void InvokeCompleteUI(Quest q)
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (currentNotif)
            {
                if (this.timeElapsedSinceBannerCreation.HasTimeElapsed(5.0f)){

                }
            }
        }
    }
}
