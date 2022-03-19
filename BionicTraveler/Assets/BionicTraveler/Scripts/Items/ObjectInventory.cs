namespace BionicTraveler.Scripts.Items
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Inventory of an object that supports the player looting it.
    /// </summary>
    public class ObjectInventory : BasicInventory
    {
        /// <inheritdoc/>
        public override bool SupportsUsage => true;

        /// <summary>
        /// Uses the passed <see cref="InventoryItem"/>.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public override void Use(InventoryItem item)
        {
            Debug.Log($"Looting {item.ItemData.DisplayName}");

            this.Remove(item.ItemData);

            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerEntity>().Inventory.AddItem(item.ItemData);
        }
    }
}