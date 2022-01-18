namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// Quest objective for reaching a certain volume. TODO: Create Volume and VolumeManager classes to
    /// allow designers to place volumes in the game map that can then be identified here via name.
    /// </summary>
    [System.Serializable]
    public class QuestObjectiveVolume : QuestObjective
    {
        [SerializeField]
        private string volumeName;

        /// <inheritdoc/>
        public override object Clone()
        {
            var objective = new QuestObjectiveVolume();
            objective.CloneBase(objective);
            objective.volumeName = this.volumeName;
            return objective;
        }
    }
}