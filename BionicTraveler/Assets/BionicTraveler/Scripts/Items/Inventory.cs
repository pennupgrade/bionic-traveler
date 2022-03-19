namespace BionicTraveler.Scripts.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Inventory class. Manage player/NPC inventory.
    /// </summary>
    public class Inventory : BasicInventory
    {
        /// <summary>
        /// The owner of the inventory.
        /// </summary>
        private readonly DynamicEntity owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="owner">The owner of the inventory.</param>
        public Inventory(DynamicEntity owner)
        {
            this.owner = owner;
        }

        /// <inheritdoc/>
        public override bool SupportsDropping => true;

        /// <inheritdoc/>
        public override bool SupportsUsage => true;

        /// <summary>
        /// Adds a new key item.
        /// </summary>
        /// <param name="key">The key</param>
        public void AddItem(ItemDataKey key)
        {
            (this.owner as PlayerEntity).KeyManager.AddKey(key);
        }

        /// <summary>
        /// Drops <see cref="item"/>.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>A pickup if the item was dropped, null otherwise.</returns>
        public override Pickup DropItem(ItemData item)
        {
            // Cannot remove the item is not in inventory.
            if (!this.ContainsItem(item))
            {
                throw new ArgumentException("Item is not in inventory.");
            }

            this.Remove(item);

            // Create pickup.
            return PickupCreator.SpawnPickup(this.owner.transform.position + (this.owner.Direction * 2.0f), item);
        }

        /// <summary>
        /// Drops all items in our inventory.
        /// </summary>
        /// <returns>An array of pickups.</returns>
        public Pickup[] DropAll()
        {
            List<Pickup> pickups = new List<Pickup>();
            foreach (var item in this.Items.ToArray())
            {
                var offset = new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f), this.owner.transform.position.z);
                pickups.Add(PickupCreator.SpawnPickup(this.owner.transform.position + offset, item.ItemData));
                this.Remove(item.ItemData);
            }

            return pickups.ToArray();
        }

        /// <summary>
        /// Uses the passed <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public override void Use(InventoryItem item)
        {
            Debug.Log($"Using {item.ItemData.DisplayName}");

            this.Remove(item.ItemData);
            item.ItemData.Interact(this.owner);
        }
    }
}