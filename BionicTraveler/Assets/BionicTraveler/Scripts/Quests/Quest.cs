namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum QuestState
    {
        None,
        Active,
        Inactive,
        Finished,
    }

    /// <summary>
    /// Please document me. Explore all Temples in the Universe.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewQuest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField]
        private List<QuestStage> stages;
        private string title;
        private string description;
        private QuestState state;

        public bool IsComplete()
        {
            return true;
        }
    }
}
