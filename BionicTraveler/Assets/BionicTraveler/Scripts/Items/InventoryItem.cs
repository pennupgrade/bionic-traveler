namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;
    using System;
    using System.Runtime.Serialization;
    using System.Reflection;
    using UnityEditor;

    /// <summary>
    /// An item inside an inventory.
    /// </summary>
    [Serializable]
    public class InventoryItem : ISerializable
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
        public ItemData ItemData { get; set; }

        /// <summary>
        /// Gets the quantity of the item.
        /// </summary>
        [SerializeField]
        public int Quantity { get; private set; }

        /// <summary>
        /// Adds a given quantity to the inventory.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>True if the item was added to the inventory, false otherwise.</returns>
        public bool Add(int quantity)
        {
            this.Quantity += quantity;

            if (this.Quantity > this.ItemData.MaxNumber)
            {
                Debug.LogWarning($"InventoryItem::Add: Attempt to add more items of type {this.ItemData.Id} than allowed.");
                this.Quantity = this.ItemData.MaxNumber;
                return false;
            }

            return true;
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

        // Implement this method to serialize data. The method is called on serialization.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("ItemData", AssetDatabase.GetAssetPath(ItemData), typeof(string));
            //info.AddValue("quantity", Quantity);
        }

        // The special constructor is used to deserialize values.
        // In this case, it recreate the original ScriptableObject.
        public InventoryItem(SerializationInfo info, StreamingContext context)
        {
            //Quantity = (int)info.GetValue("quantity", typeof(int));
            //ItemData = (ItemData)AssetDatabase.LoadAssetAtPath((string)info.GetValue("ItemData", typeof(string)), typeof(ItemData));
        }
    }
}