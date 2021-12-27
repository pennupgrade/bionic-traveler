namespace BionicTraveler
{
    using BionicTraveler.Scripts.Items;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Renders a given inventory slot based on <see cref="InventoryItem"/>.
    /// </summary>
    public class InventorySlot : MonoBehaviour
    {
        private Inventory inventory;
        private InventoryUI owner;
        private InventoryItem item;

        [SerializeField]
        private Image icon;
        [SerializeField]
        private Button removeButton;
        [SerializeField]
        private Text quantityText;

        /// <summary>
        /// Sets the item to use for this slot.
        /// </summary>
        /// <param name="owner">The owning inventory UI.</param>
        /// <param name="item">The item.</param>
        public void SetItem(InventoryUI owner, InventoryItem item)
        {
            this.owner = owner;
            this.inventory = owner.Inventory;
            this.item = item;
            this.icon.sprite = item.ItemData.InventorySprite != null
                ? item.ItemData.InventorySprite : owner.PlaceholderSprite;
            this.icon.enabled = true;
            this.removeButton.interactable = true;
            this.quantityText.enabled = true;
            this.quantityText.text = item.Quantity.ToString();
        }

        /// <summary>
        /// Uses the inventory item.
        /// </summary>
        public void UseItem()
        {
            if (this.item != null)
            {
                this.inventory.Use(this.item);
            }
        }

        /// <summary>
        /// Removes the item in this slot.
        /// </summary>
        public void RemoveItem()
        {
            this.item = null;
            this.icon.sprite = null;
            this.icon.enabled = false;
            this.removeButton.interactable = false;
            this.quantityText.enabled = false;
        }

        /// <summary>
        /// Called when the user interacted with the remove button.
        /// </summary>
        public void OnRemoveButton()
        {
            this.inventory.DropItem(this.item.ItemData);
        }
    }
}
