namespace BionicTraveler.Scripts.Items
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    public class WeaponsInventory
    {
        private readonly DynamicEntity owner;
        public WeaponData WeaponData;

        private WeaponData equippedWeapon;
        public WeaponBehaviour equippedWeaponBehavior;
        private GameObject weaponWorldObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponsInventory"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public WeaponsInventory(DynamicEntity owner)
        {
            this.owner = owner;
        }

        public bool IsWeaponVisible => this.weaponWorldObject.activeSelf;

        /// <summary>
        /// Gets a value indicating whether the currently equipped weapon is unarmed.
        /// </summary>
        public bool IsUnarmed => this.equippedWeaponBehavior.IsUnarmed;

        public void AddWeapon(WeaponData weaponData)
        {
            this.WeaponData = weaponData;

            this.equippedWeapon = this.WeaponData;
            this.weaponWorldObject = this.Equip();
            this.equippedWeaponBehavior = this.weaponWorldObject.AddComponent<WeaponBehaviour>();
            this.equippedWeaponBehavior.SetData(this.equippedWeapon);
            this.equippedWeaponBehavior.SetWorldObject(this.weaponWorldObject);
            this.HideCurrentWeapon();

            // TODO: If we ever change our weapon
            //GameObject.Destroy(this.weaponWorldObject);
            //this.equippedWeapon = null;
            //this.weaponWorldObject = null;
            //this.equippedWeaponBehavior = null;
        }

        public void ToggleWeaponVisibility()
        {
            if (!this.IsWeaponVisible)
            {
                this.DisplayCurrentWeapon();
            }
            else
            {
                this.HideCurrentWeapon();
            }
        }

        public void DisplayCurrentWeapon()
        {
            this.weaponWorldObject.SetActive(true);
        }

        public void HideCurrentWeapon()
        {
            this.weaponWorldObject.SetActive(false);
        }

        /// <summary>
        /// Equips this weapon behavior by creating its physical reprensentation in the world.
        /// </summary>
        /// <returns>The physical object reprensentation.</returns>
        private GameObject Equip()
        {
            this.weaponWorldObject = GameObject.Instantiate(this.WeaponData.WorldObjectPrefab);
            this.weaponWorldObject.transform.SetParent(this.owner.transform, false);
            return this.weaponWorldObject;
        }
    }
}