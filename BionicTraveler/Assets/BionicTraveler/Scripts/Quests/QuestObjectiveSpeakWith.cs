namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Linq;
    using BionicTraveler.Scripts.Items;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Quest objective for collecting items. TODO: Make applicable for NPC inventories too?
    /// </summary>
    [System.Serializable]
    public class QuestObjectiveSpeakWith : QuestObjective
    {
        [SerializeField]
        private String targetEntityName;

        /// <inheritdoc/>
        public override void Activate()
        {
            base.Activate();
        }

        public override object Clone()
        {
            var objective = new QuestObjectiveSpeakWith();
            objective.CloneBase(objective);
            objective.targetEntityName = this.targetEntityName;
            return objective;
        }

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEventInventory">The event.</param>
        public void ProcessEvent(QuestEventSpokenTo questEventSpokenTo)
        {
            //Debug.Log();
            this.CheckTargetName(questEventSpokenTo.personSpokenTo);
        }

        private void CheckTargetName(String name)
        {
            if (name == this.targetEntityName)
            {
                Debug.Log("Complete Quest: Spoken to " + name + "!");
                this.SetAsComplete();
            }
            else
            {
                Debug.Log("Did not speak with target person");
                this.SetAsIncomplete();
            }
        }
    }
}
