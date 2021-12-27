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
        private Inventory inventory;
        private InventorySlot[] slots;

        [SerializeField]
        private Transform itemsParent;

        /// <summary>
        /// Gets an instance of the InventoryUI class -> used to make sure that there is the only one instance of MapMenu.
        /// </summary>
        public static InventoryUI Instance { get; private set; }

        /// <inheritdoc/>
        public override void Start()
        {
            base.Start();

            // TODO: Refactor to be hooked up elsewhere for the player so we can support multiple
            // owners/inventories.
            var player = GameObject.FindGameObjectWithTag("Player");
            this.SetInventoryData(player.GetComponent<DynamicEntity>().Inventory);

            this.slots = this.itemsParent.GetComponentsInChildren<InventorySlot>();
        }

        /// <summary>
        /// Sets the inventory data to use.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        public void SetInventoryData(Inventory inventory)
        {
            this.inventory = inventory;
            this.inventory.ItemsChanged += this.Inventory_ItemsChanged;
        }

        private void Inventory_ItemsChanged(Inventory sender, InventoryItem item)
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
                    this.slots[i].SetItem(this.inventory, items[i]);
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
