namespace BionicTraveler.Scripts.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Represents a basic inventory of items that can be associated with objects such as treasure chests.
    /// Additional functionality, such as item usage and pickups is implemented in <see cref="Inventory"/>.
    /// </summary>
    public class BasicInventory
    {
        /// <summary>
        /// Item container. Stores the items and their counts (for stackable items)
        /// Change the comparator later to define the order of the items.
        /// </summary>
        private readonly SortedDictionary<ItemData, InventoryItem> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicInventory"/> class.
        /// </summary>
        public BasicInventory()
        {
            this.items = new SortedDictionary<ItemData, InventoryItem>();
        }

        /// <summary>
        /// The delegate for inventory changes.
        /// </summary>
        /// <param name="sender">The sender, i.e. the inventory.</param>
        /// <param name="item">The item.</param>
        public delegate void InventoryChangedEventHandler(BasicInventory sender, InventoryItem item);

        /// <summary>
        /// Called whenever the items collection has changed.
        /// </summary>
        public event InventoryChangedEventHandler ItemsChanged;

        /// <summary>
        /// Gets a read only copy collection of the inventory items.
        /// </summary>
        public IReadOnlyCollection<InventoryItem> Items => this.items.Values.ToArray();

        /// <summary>
        /// Gets a value indicating whether this inventory supports dropping items.
        /// </summary>
        public virtual bool SupportsDropping => false;

        /// <summary>
        /// Gets a value indicating whether this inventory supports using items.
        /// </summary>
        public virtual bool SupportsUsage => false;

        /// <summary>
        /// Adds <see cref="Item"/> to the inventory.
        /// </summary>
        /// <param name="item">Item to insert.</param>
        ///<returns>True if the item was added to the inventory, false otherwise.</returns>
        public bool AddItem(ItemData item)
        {
            //Debug.Log("Inventory received item");
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
        /// Adds all items from <paramref name="lootTable"/> to this inventory. The loot table remains unchanged.
        /// </summary>
        /// <param name="lootTable">The loot table.</param>
        public void AddFromLootTable(LootTable lootTable)
        {
            foreach (var item in lootTable.Items)
            {
                this.AddItem(item);
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
        /// Removes a given item from the inventory.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(InventoryItem item)
        {
            this.Remove(item.ItemData);
        }

        /// <summary>
        /// Removes a given item data from the inventory.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void Remove(ItemData item)
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
        /// Drops <see cref="item"/>.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>A pickup if the item was dropped, null otherwise.</returns>
        public virtual Pickup DropItem(ItemData item)
        {
            Debug.Log("BasicInventory::DropItem: Pure virt");
            return null;
        }

        /// <summary>
        /// Uses the passed <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public virtual void Use(InventoryItem item)
        {
            Debug.Log("BasicInventory::Use: Pure virt");
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