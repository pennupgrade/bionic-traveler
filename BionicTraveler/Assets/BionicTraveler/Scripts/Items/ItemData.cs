namespace BionicTraveler.Scripts.Items
{
    using System;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes an item that can be used in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewItem", menuName = "Items/ItemData", order = 0)]
    public class ItemData : ScriptableObject, IEquatable<ItemData>
    {
        [SerializeField]
        [Tooltip("The unique internal identifier of the item. This can be used by scripts to query item information.")]
        private string id;

        [SerializeField]
        [Tooltip("The name displayed in the inventory.")]
        private string displayName;

        [SerializeField]
        [Tooltip("The description displayed in the inventory.")]
        private string description;

        [SerializeField]
        [Tooltip("The maximum number of items of this type that can be carried.")]
        private int maxNumber;

        [SerializeField]
        [Tooltip("The sprite displayed in the inventory.")]
        private Sprite inventorySprite;

        [SerializeField]
        [Tooltip("The sprite used for pickups based on this item.")]
        private Sprite pickupSprite; // Perhaps refactor to PickupData class?

        [SerializeField]
        [Tooltip("The pick up range for this item.")]
        private float pickupRange;

        [SerializeField]
        [Tooltip("The type of the item.")]
        private ItemType type;

        [SerializeReference]
        [Tooltip("The consumable used when this item is interacted with in the inventory.")]
        private Consumable consumable;

        [SerializeReference]
        [Tooltip("The equippable used when this item is interacted with in the inventory.")]
        private Equippable equippable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemData"/> class.
        /// </summary>
        public ItemData()
        {
            if (this.type == ItemType.Consumable && this.consumable == null)
            {
                throw new ArgumentException($"Item {this.id} of type consumable needs a consumable configured.");
            }

            if (this.type == ItemType.Equippable && this.equippable == null)
            {
                throw new ArgumentException($"Item {this.id} of type equippable needs a equippable configured.");
            }
        }

        /// <summary>
        /// Gets the item's unique identifier.
        /// </summary>
        public string Id => this.id;

        /// <summary>
        /// Gets the display name of the item.
        /// </summary>
        public string DisplayName => this.displayName;

        /// <summary>
        /// Gets the description of the item.
        /// </summary>
        public string Description => this.description;

        /// <summary>
        /// Gets the maximum number of items of this type that can be carried.
        /// </summary>
        public int MaxNumber => this.maxNumber;

        /// <summary>
        /// Gets the sprite displayed in the inventory.
        /// </summary>
        public Sprite InventorySprite => this.inventorySprite;

        /// <summary>
        /// Gets the sprite displayed in the game world.
        /// </summary>
        public Sprite PickupSprite => this.pickupSprite;

        /// <summary>
        /// Gets the range from where this item can be picked up.
        /// </summary>
        public float PickupRange => this.pickupRange;

        /// <summary>
        /// Gets a value indicating whether this item can be used, that is it is a consumable or equippable.
        /// </summary>
        public bool CanBeInteractedWith => this.type == ItemType.Consumable || this.type == ItemType.Equippable;

        /// <summary>
        /// Creates a new pickup in the game world representing this item.
        /// </summary>
        /// <param name="position">The world position to create the item at.</param>
        /// <returns>A newly created pickup.</returns>
        public Pickup CreatePickup(Vector3 position)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Interacts with the item by calling its respective interaction functionality.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Interact(Entity entity)
        {
            if (!this.CanBeInteractedWith)
            {
                throw new InvalidOperationException("Item cannot be interacted with.");
            }

            if (this.type == ItemType.Consumable)
            {
                this.consumable.Use(entity);
            }
            else if (this.type == ItemType.Equippable)
            {
                if (entity is DynamicEntity dynamicEntity)
                {
                    // TODO: Select replacement equippable and slot.
                    this.equippable.Equip(dynamicEntity);
                }
                else
                {
                    throw new ArgumentException($"{nameof(entity)} is not a dynamic entity.");
                }
            }
        }

        /// <inheritdoc/>
        public bool Equals(ItemData other)
        {
            if (other == null)
            {
                return false;
            }

            return this.id == other.id;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "More concise.")]
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals(obj as ItemData);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (Application.isEditor)
            {
                // Needed so editor does not throw exceptions when this is not set.
                if (this.id == null)
                {
                    return 0;
                }
            }

            return this.id.GetHashCode();
        }
    }
}
