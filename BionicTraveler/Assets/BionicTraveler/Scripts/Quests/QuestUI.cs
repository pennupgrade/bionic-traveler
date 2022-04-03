namespace BionicTraveler.Scripts.Quests
{
    using BionicTraveler.Assets.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        public GameObject notifPrefab;

        public GameObject currentNotif;

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
            this.listCompleteQuests = new List<Quest>();
        }

        private void InvokeCompleteUI(Quest q)
        {
            this.currentNotif = GameObject.Instantiate(this.notifPrefab, this.questManager.transform.position, Quaternion.identity);
            TextMeshProUGUI t = this.currentNotif.transform.GetChild(0).transform.GetComponentInChildren<TextMeshProUGUI>();
            t.SetText(q.getTitle() + " has been completed!");
            this.listCompleteQuests.Remove(q);

            this.currentNotif.transform.parent = this.transform;
            this.timeElapsedSinceBannerCreation = GameTime.Now;
        }

        public void AddToCompleteList(Quest q)
        {
            this.listCompleteQuests.Add(q);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (this.currentNotif)
            {
                if (this.timeElapsedSinceBannerCreation.HasTimeElapsed(5.0f)) {
                    Destroy(this.currentNotif);
                }
            } else
            {
                if (this.listCompleteQuests.Count > 0)
                {
                    Quest firstQuest = this.listCompleteQuests[0];
                    this.InvokeCompleteUI(firstQuest);
                }
            }
        }
    }
}
