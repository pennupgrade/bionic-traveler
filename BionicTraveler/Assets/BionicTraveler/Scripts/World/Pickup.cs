namespace BionicTraveler.Scripts.World
{
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Represents a pickup in the game world. Configure <see cref="ItemData"/> to set the item to be picked up.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        [Tooltip("The associated item to be picked up.")]
        [SerializeField]
        private ItemData itemData;

        [Tooltip("Whether or not this item should be updated whenever its template changes.")]
        [SerializeField]
        private bool keepSyncedWithTemplate;

        [Tooltip("The sound to play when the item is being picked up.")]
        [SerializeField]
        private AudioClip pickupSound;

        private float pickUpRange;
        private bool hasBeenPickedUp;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Pickup"/> should be synced with its
        /// <see cref="ItemData"/>.
        /// </summary>
        public bool KeepSyncedWithTemplate => this.keepSyncedWithTemplate;

        /// <summary>
        /// Gets the associated item data.
        /// </summary>
        public ItemData ItemData => this.itemData;

        /// <summary>
        /// Gets the pick up range.
        /// </summary>
        public float PickUpRange => this.pickUpRange;

        /// <summary>
        /// Gets a value indicating whether this Pickup has been collected.
        /// </summary>
        public bool HasBeenPickedUp => this.hasBeenPickedUp;

        // Awake is called when the script instance is being loaded.
        private void Start()
        {
            Debug.Log("calling start for new pickup, " + this.ItemData);
            this.pickUpRange = this.ItemData.PickupRange;
        }

        /// <summary>
        /// Sets the associated item from code and updates all properties based on the template.
        /// </summary>
        /// <param name="itemData">The item.</param>
        public void SetItemData(ItemData itemData)
        {
            this.itemData = itemData;
            this.InitializeFromTemplate();
        }

        /// <summary>
        /// Initializes class data based on the associated <see cref="ItemData"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Editor stuff comes last.")]
        public void InitializeFromTemplate()
        {
            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = this.itemData?.PickupSprite;
            }
            else
            {
                Debug.LogWarning($"Item {this.name} does not have a SpriteRenderer!");
            }
        }

        /// <summary>
        /// Picks up this pickup and adds it to the inventory of the specified entity.
        /// </summary>
        /// <param name="entity">The entity that picked up this pickup.</param>
        public void PickUp(DynamicEntity entity)
        {
            Debug.Log("Item has been picked up");
            this.hasBeenPickedUp = true;
            entity.Inventory.AddItem(this.itemData);

            if (this.pickupSound != null)
            {
                AudioManager.Instance.PlayOneShot(this.pickupSound);
            }
            else
            {
                Debug.LogWarning("Pickup::Pickup: No pickup sound configured!");
            }

            // TODO: We should destroy ourselves at some point once we have been picked up.
            // But it might be useful for other scripts to be able to ask us if we have been picked up.
            // Maybe use an event system?
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        /// </summary>
        private void OnValidate()
        {
            if (this.KeepSyncedWithTemplate)
            {
                // Queue call to call it after all inspectors have updated.
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (!this.IsDestroyed())
                    {
                        this.InitializeFromTemplate();
                    }
                };
            }
        }
    }
}
