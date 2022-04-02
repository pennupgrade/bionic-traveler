namespace BionicTraveler.Scripts.Quests
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

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEventKilled">The event.</param>
        public void ProcessEvent(QuestEventEnemyKilled questEventKilled)
        {
            if (this.enemyName == questEventKilled.EnemyName)
            {
                this.CheckHasKilledEnough();
            }
            else
            {
                Debug.Log("killed wrong type of enemy");
            }
        }

        private void CheckHasKilledEnough()
        {
            this.killCount--;
            if (this.killCount < 1)
            {
                Debug.Log("killed enough of the enemy");
                this.SetAsComplete();
            }
        }

    }
}
