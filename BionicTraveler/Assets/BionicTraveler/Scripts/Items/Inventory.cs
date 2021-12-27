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
    public class Inventory
    {
        /// <summary>
        /// The owner of the inventory.
        /// </summary>
        private readonly DynamicEntity owner;

        /// <summary>
        /// Item container. Stores the items and their counts (for stackable items)
        /// Change the comparator later to define the order of the items.
        /// </summary>
        private readonly SortedDictionary<ItemData, InventoryItem> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="owner">The owner of the inventory.</param>
        public Inventory(DynamicEntity owner)
        {
            this.owner = owner;
            this.items = new SortedDictionary<ItemData, InventoryItem>();
        }

        /// <summary>
        /// The delegate for inventory changes.
        /// </summary>
        /// <param name="sender">The sender, i.e. the inventory.</param>
        /// <param name="item">The item.</param>
        public delegate void InventoryChangedEventHandler(Inventory sender, InventoryItem item);

        /// <summary>
        /// Called whenever the items collection has changed.
        /// </summary>
        public event InventoryChangedEventHandler ItemsChanged;

        /// <summary>
        /// Gets a read only copy collection of the inventory items.
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items => this.items.Values.ToArray();

        /// <summary>
        /// Adds <see cref="Item"/> to the inventory.
        /// </summary>
        /// <param name="item">Item to insert.</param>
        ///<returns>True if the item was added to the inventory, false otherwise.</returns>
        public bool AddItem(ItemData item)
        {
            Debug.Log("Inventory received item");
            if (item.Type == ItemType.Key)
            {
                this.AddItem((ItemDataKey)item);
                return true;
            }

            if (this.items.ContainsKey(item))
            {
                if (this.items[item].Add(1))
                {
                    this.ItemsChanged?.Invoke(this, this.items[item]);
                }
            }
            else
            {
                this.items[item] = new InventoryItem(item);
                this.ItemsChanged?.Invoke(this, this.items[item]);
            }

            return true;
        }

        /// <summary>
        /// Adds a new key item.
        /// </summary>
        /// <param name="key">The key</param>
        public void AddItem(ItemDataKey key)
        {
            (this.owner as PlayerEntity).KeyManager.AddKey(key);
        }

        /// <summary>
        /// Checks whether this inventory contains an item with the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>True if the inventory contains the item, false otherwise.</returns>
        public bool ContainsItem(string identifier)
        {
            return this.items.Keys.Any(item => item.Id == identifier);
        }

        /// <summary>
        /// Checks whether this inventory contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the inventory contains the item, false otherwise.</returns>
        public bool ContainsItem(ItemData item)
        {
            return this.items.ContainsKey(item);
        }

        /// <summary>
        /// Drops <see cref="item"/>.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>A pickup if the item was dropped, null otherwise.</returns>
        public Pickup DropItem(ItemData item)
        {
            // Cannot remove the item is not in inventory.
            if (!this.items.ContainsKey(item))
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
            foreach (var item in this.items.ToArray())
            {
                var offset = new Vector3(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f), this.owner.transform.position.z);
                pickups.Add(PickupCreator.SpawnPickup(this.owner.transform.position + offset, item.Key));
                this.Remove(item.Key);
            }

            return pickups.ToArray();
        }

        /// <summary>
        /// Removes a given item from the inventory.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(InventoryItem item)
        {
            this.Remove(item.ItemData);
        }

        private void Remove(ItemData item)
        {
            // Decrement to drop/consume.
            this.items[item].Remove(1);

            var itemRemoved = this.items[item];
            if (this.items[item].Quantity == 0)
            {
                // If there's none of that item remaining, remove altogether.
                this.items.Remove(item);
            }

            this.ItemsChanged?.Invoke(this, itemRemoved);
        }

        /// <summary>
        /// Uses the passed <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public void Use(InventoryItem item)
        {
            Debug.Log($"Using {item.ItemData.DisplayName}");

            this.Remove(item.ItemData);
            item.ItemData.Interact(this.owner);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string result = string.Empty;
            foreach (var item in this.items)
            {
                result += $"{item.Key.DisplayName}, {item.Value.Quantity} \n";
            }

            return !string.IsNullOrWhiteSpace(result) ? result : "Inventory is empty";
        }
    }
}