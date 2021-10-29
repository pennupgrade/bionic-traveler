namespace BionicTraveler.Scripts.Items
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Links weapon data to an equippable. This is necessary since the Unity Inspector does not
    /// play nice with interface referneces, so we have to make things slightly more complicated
    /// and duplicate the equippable for each weapon data. The advantage is a very clean separation
    /// between weapon data, behavior, item and equippable.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewWeaponEquippable", menuName = "Items/Equippables/Weapon")]
    public class WeaponEquippable : Equippable
    {
        [SerializeField]
        [Tooltip("The associated weapon to be equipped.")]
        private WeaponData weaponData;

        [SerializeField]
        [Tooltip("The associated item to be put into the entity's inventory when this weapon is unequiped. " +
            "Overrides the item from the weapon data.")]
        private ItemData itemDataOverride;

        /// <inheritdoc/>
        public override void Equip(Entity entity)
        {
            // Get from entity gameobject.
            CombatBehavior combatBehavior = null;
            combatBehavior.SetWeapon(this.weaponData);
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Unequip(Entity entity)
        {
            // Get entity inventory and add us back to it!
            var itemData = this.itemDataOverride ?? this.weaponData.ItemData;
            Inventory inventory = null;
            inventory.AddItem(itemData);

            CombatBehavior combatBehavior = null;
            combatBehavior.RemoveWeapon();

            throw new System.NotImplementedException();
        }
    }
}