namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Manages combat behavior of the currently equipped weapon.
    /// </summary>
    public class CombatBehavior : MonoBehaviour
    {
        private WeaponBehavior weaponBehavior;
        private List<WeaponBehavior> dormantWeapons;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatBehavior"/> class.
        /// </summary>
        public CombatBehavior()
        {
            this.dormantWeapons = new List<WeaponBehavior>();
        }

        /// <summary>
        /// Sets the currently equipped weapon.
        /// </summary>
        /// <param name="weaponData">The new weapon.</param>
        public void SetWeapon(WeaponData weaponData)
        {
            if (weaponData is null)
            {
                throw new ArgumentNullException(nameof(weaponData));
            }

            this.RemoveCurrentWeapon();

            // Add new behavior.
            var newWeapon = this.gameObject.AddComponent<WeaponBehavior>();
            newWeapon.SetData(weaponData);
            this.weaponBehavior = newWeapon;
        }

        /// <summary>
        /// Removes the currently equipped weapon.
        /// </summary>
        public void RemoveWeapon()
        {
            this.RemoveCurrentWeapon();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            if (this.weaponBehavior != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    this.weaponBehavior.Fire();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    this.weaponBehavior.SwitchWeaponMode();
                }
            }

            // Destroy weapon behaviors that have no attacks left on scene.
            foreach (var weapon in this.dormantWeapons)
            {
                if (weapon.HaveAllAttacksBeenDisposed)
                {
                    Destroy(weapon);
                }
            }

            this.dormantWeapons = this.dormantWeapons.Where(weapon => !weapon.HaveAllAttacksBeenDisposed).ToList();
        }

        private void RemoveCurrentWeapon()
        {
            // Add our current weapon behavior to a list of all weapon behaviors that we can try to free.
            if (this.weaponBehavior != null)
            {
                this.dormantWeapons.Add(this.weaponBehavior);
                this.weaponBehavior.enabled = false;
            }

            this.weaponBehavior = null;
        }
    }
}
