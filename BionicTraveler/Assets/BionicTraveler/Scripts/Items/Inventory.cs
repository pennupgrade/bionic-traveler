namespace BionicTraveler.Scripts.Items
{
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
        /// Gets a read only copy collection of the inventory items.
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items => this.items.Values.ToArray();

        /// <summary>
        /// Adds <see cref="Item"/> to the inventory.
        /// </summary>
        /// <param name="item">Item to insert.</param>
        public void AddItem(ItemData item)
        {
            Debug.Log("Inventory received item");
            if (this.items.ContainsKey(item))
            {
                this.items[item].Add(1);
            }
            else
            {
                this.items[item] = new InventoryItem(item);
            }
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
        /// <param name="position">Position to drop the item.</param>
        /// <returns>A pickup if the item was dropped, null otherwise.</returns>
        public Pickup DropItem(ItemData item, Vector3 position)
        {
            // TODO: We could make the inventory aware of its owner (entity) or pass an entity to calculate
            // an appropriate drop off position.

            // Cannot remove the item is not in inventory.
            if (!this.items.ContainsKey(item))
            {
                // TODO: Maybe throw exception?
                return null;
            }

            this.Remove(item);
            return item.CreatePickup(position);
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

            if (this.items[item].Quantity == 0)
            {
                // If there's none of that item remaining, remove altogether.
                this.items.Remove(item);
            }
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