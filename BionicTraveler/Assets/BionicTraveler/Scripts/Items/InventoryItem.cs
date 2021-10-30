namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;

    /// <summary>
    /// An item inside an inventory.
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryItem"/> class.
        /// </summary>
        /// <param name="itemData">The item data.</param>
        public InventoryItem(ItemData itemData)
        {
            this.ItemData = itemData;
            this.Quantity = 1;
        }

        /// <summary>
        /// Gets the item data.
        /// </summary>
        public ItemData ItemData { get; }

        /// <summary>
        /// Gets the quantity of the item.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Adds a given quantity to the inventory.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        public void Add(int quantity)
        {
            this.Quantity += quantity;

            if (this.Quantity > this.ItemData.MaxNumber)
            {
                Debug.LogWarning($"InventoryItem::Add: Attempt to add more items of type ${this.ItemData.Id} than allowed.");
                this.Quantity = this.ItemData.MaxNumber;
            }
        }

        /// <summary>
        /// Removes a given quantity from the inventory.
        /// </summary>
        /// <param name="quantity">The quantity to remove.</param>
        public void Remove(int quantity)
        {
            this.Quantity -= quantity;
            if (this.Quantity < 0)
            {
                Debug.LogWarning($"InventoryItem::Remove: Attempt to remove more items of type ${this.ItemData.Id} than in inventory.");
                this.Quantity = 0;
            }
        }
    }
}