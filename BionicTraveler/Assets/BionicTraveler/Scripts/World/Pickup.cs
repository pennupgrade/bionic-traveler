namespace BionicTraveler.Scripts.World
{
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

        // This is just a placeholder until we have an actual player entity.
        private Transform player;
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
        /// Gets a value indicating whether this Pickup has been collected.
        /// </summary>
        public bool HasBeenPickedUp => this.hasBeenPickedUp;

        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            this.pickUpRange = this.itemData.PickupRange;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            // Check if player is in range and "E" is pressed
            Vector3 distanceToPlayer = this.player.position - this.transform.position;
            if (!this.hasBeenPickedUp && distanceToPlayer.magnitude <= this.pickUpRange && Input.GetKeyDown(KeyCode.E))
            {
                this.hasBeenPickedUp = true;

                // This is just an example, we do not actually have an inventory from the player yet. TODO: Fix.
                Inventory inventory = null;
                inventory.AddItem(this.itemData);

                // TODO: We should destroy ourselves at some point once we have been picked up.
                // But it might be useful for other scripts to be able to ask us if we have been picked up.
                // Maybe use an event system?
                this.gameObject.SetActive(false);
            }
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
