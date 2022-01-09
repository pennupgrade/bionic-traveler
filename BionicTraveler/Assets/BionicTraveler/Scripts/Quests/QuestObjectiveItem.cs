namespace BionicTraveler.Scripts.Quests
{
    using BionicTraveler.Scripts.Items;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class QuestObjectiveItem : QuestObjective
    {
        [SerializeField]
        private ItemData targetItem;

        [SerializeField]
        private int itemCount;

        public override bool IsComplete()
        {
            return base.IsComplete();
        }
    }
}
