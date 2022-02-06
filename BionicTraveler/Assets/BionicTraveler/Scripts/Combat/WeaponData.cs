namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// Describes a weapon that can be used in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewWeapon", menuName = "Attacks/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The unique internal identifier of the weapon. This can be used by scripts to query weapon information.")]
        private string id;

        [SerializeField]
        [Tooltip("The name of the weapon.")]
        private string displayName;

        [SerializeField]
        [Tooltip("The sprite of the weapon.")]
        private Sprite sprite;

        [SerializeField]
        [Tooltip("The object prefab that represents this weapon in the game world.")]
        private GameObject worldObjectPrefab;

        [SerializeField]
        [Tooltip("The primary attack of the weapon.")]
        private AttackData primaryAttackData;

        [SerializeField]
        [Tooltip("The secondary attack of the weapon, if any.")]
        private AttackData secondaryAttackData;

        [SerializeField]
        [Tooltip("The associated item that represents this weapon in an entity's inventory when this weapon is unequiped.")]
        private ItemData itemData;

        /// <summary>
        /// Gets the weapon's unique identifier.
        /// </summary>
        public string Id => this.id;

        /// <summary>
        /// Gets the display name of the weapon.
        /// </summary>
        public string DisplayName => this.displayName;

        /// <summary>
        /// Gets the weapon's sprite.
        /// </summary>
        public Sprite Sprite => this.sprite;

        /// <summary>
        /// Gets the world object prefab this weapon uses.
        /// </summary>
        public GameObject WorldObjectPrefab => this.worldObjectPrefab;

        /// <summary>
        /// Gets the primary attack data used by the weapon.
        /// </summary>
        public AttackData PrimaryAttackData => this.primaryAttackData;

        /// <summary>
        /// Gets the secondary attack data used by the weapon, if any.
        /// </summary>
        public AttackData SecondaryAttackData => this.secondaryAttackData;

        /// <summary>
        /// Gets the item that represents this weapon in an entity's inventory.
        /// </summary>
        public ItemData ItemData => this.ItemData;
    }
}
