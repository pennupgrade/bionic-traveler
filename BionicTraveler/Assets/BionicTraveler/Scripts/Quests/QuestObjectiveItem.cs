namespace BionicTraveler.Scripts.Quests
{
    using System.Linq;
    using BionicTraveler.Scripts.Items;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Quest objective for collecting items. TODO: Make applicable for NPC inventories too?
    /// </summary>
    [System.Serializable]
    public class QuestObjectiveItem : QuestObjective
    {
        [SerializeField]
        private ItemData targetItem;

        [SerializeField]
        private int itemCount;

        /// <inheritdoc/>
        public override void Activate()
        {
            base.Activate();
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerEntity = player.GetComponent<PlayerEntity>();
            this.CheckInventory(playerEntity.Inventory);
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            var objective = new QuestObjectiveItem();
            objective.CloneBase(objective);
            objective.targetItem = this.targetItem;
            objective.itemCount = this.itemCount;
            return objective;
        }

        /// <summary>
        /// Processes a new quest event. Use specific overloads to only process certain events.
        /// Calls are made from <see cref="QuestStage.ProcessEvent(QuestEvent)"/> through double
        /// dispatching to select the correct overload.
        /// </summary>
        /// <param name="questEventInventory">The event.</param>
        public void ProcessEvent(QuestEventInventory questEventInventory)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var playerEntity = player.GetComponent<PlayerEntity>();
            this.CheckInventory(playerEntity.Inventory);
        }

        private void CheckInventory(Inventory inventory)
        {
            if (inventory.Items.Any(i => i.ItemData == this.targetItem && i.Quantity >= this.itemCount))
            {
                Debug.Log("Got all items!");
                this.SetAsComplete();
            }
            else
            {
                this.SetAsIncomplete();
            }
        }
    }
}
