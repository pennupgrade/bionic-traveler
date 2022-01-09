namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// Explore the Temple in the Wilderness.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewQuestStage", menuName = "Quests/QuestStage")]
    public class QuestStage : ScriptableObject
    {
        [SerializeField]
        private QuestObjectiveFactory objective;

        [SerializeField]
        private string description;

        private QuestObjective questObjective;

        public QuestStage()
        {
            this.questObjective = this.objective.CreateObjective();
        }

        public QuestObjectiveFactory GetFactory() => this.objective;

        public bool IsComplete()
        {
            return this.questObjective.IsComplete();
        }
    }
}
