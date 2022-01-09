namespace BionicTraveler.Scripts.Quests
{
    using BionicTraveler.Scripts.Items;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class QuestObjectiveKill : QuestObjective
    {
        [SerializeField]
        private string enemyName;

        [SerializeField]
        private int killCount;

        public override bool IsComplete()
        {
            return base.IsComplete();
        }
    }
}
