namespace BionicTraveler
{
    using System.Linq;
    using BionicTraveler.Scripts;
    using BionicTraveler.Scripts.Items;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Renders items inside a given <see cref="Inventory"/>.
    /// </summary>
    public class InventoryUI : Menu
    {
        private BasicInventory inventory;
        private InventorySlot[] slots;

        [SerializeField]
        private Transform itemsParent;

        [SerializeField]
        private Sprite placeholderSprite;

        /// <summary>
        /// Gets an instance of the InventoryUI class -> used to make sure that there is the only one instance of MapMenu.
        /// </summary>
        public static InventoryUI Instance { get; private set; }

        /// <summary>
        /// Gets the associated inventory.
        /// </summary>
        public BasicInventory Inventory => this.inventory;

        /// <summary>
        /// Gets the placeholder sprite for items.
        /// </summary>
        public Sprite PlaceholderSprite => this.placeholderSprite;

        /// <inheritdoc/>
        public override void Start()
        {
            base.Start();

            this.slots = this.itemsParent.GetComponentsInChildren<InventorySlot>();
            this.SetUsePlayerInventory();
        }

        /// <summary>
        /// Sets the inventory data to use the player's current inventory.
        /// </summary>
        public void SetUsePlayerInventory()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            this.SetInventoryData(player.GetComponent<DynamicEntity>().Inventory);
        }

        /// <summary>
        /// Sets the inventory data to use.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        public void SetInventoryData(BasicInventory inventory)
        {
            if (this.inventory != null)
            {
                this.inventory.ItemsChanged -= this.Inventory_ItemsChanged;
                this.inventory = null;
            }

            this.inventory = inventory;
            this.inventory.ItemsChanged += this.Inventory_ItemsChanged;
            this.UpdateUI();
        }

        private void Inventory_ItemsChanged(BasicInventory sender, InventoryItem item)
        {
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            var items = this.inventory.Items.ToArray();
            for (int i = 0; i < this.slots.Length; i++)
            {
                if (i < items.Length)
                {
                    this.slots[i].SetItem(this, items[i]);
                }
                else
                {
                    this.slots[i].RemoveItem();
                }
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
