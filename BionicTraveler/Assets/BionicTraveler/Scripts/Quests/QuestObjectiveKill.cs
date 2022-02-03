﻿namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// Quest objective for killing enemies. TODO: Implement main logic, perhaps via tags or entity data.
    /// </summary>
    [System.Serializable]
    public class QuestObjectiveKill : QuestObjective
    {
        [SerializeField]
        private string enemyName;

        [SerializeField]
        private int killCount;

        /// <inheritdoc/>
        public override object Clone()
        {
            var objective = new QuestObjectiveKill();
            objective.CloneBase(objective);
            objective.enemyName = this.enemyName;
            objective.killCount = this.killCount;
            return objective;
        }
    }
}