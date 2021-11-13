namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Manages combat behavior of the currently equipped weapon.
    /// </summary>
    public class CombatBehaviour : MonoBehaviour
    {
        private WeaponBehaviour weaponBehaviour;
        private List<WeaponBehaviour> dormantWeapons;

        [SerializeField]
        [Tooltip("The weapon data to use.")]
        private WeaponData firstWeaponData;

        [SerializeField]
        [Tooltip("The second weapon data to use.")]
        private WeaponData secondWeaponData;

        private bool usingFirstWeaponData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatBehaviour"/> class.
        /// </summary>
        public CombatBehaviour()
        {
            this.dormantWeapons = new List<WeaponBehaviour>();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.InitWeapons(this.firstWeaponData, this.secondWeaponData);
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
            var newWeapon = this.gameObject.AddComponent<WeaponBehaviour>();
            newWeapon.SetData(weaponData);
            this.weaponBehaviour = newWeapon;
        }

        /// <summary>
        /// Removes the currently equipped weapon.
        /// </summary>
        public void RemoveWeapon()
        {
            this.RemoveCurrentWeapon();
        }

        /// <summary>
        /// Initializes the two WeaponData options.
        /// </summary>
        /// <param name="weaponData1">The first weapon data.</param>
        /// <param name="weaponData2">The second weapon data.</param>
        public void InitWeapons(WeaponData weaponData1, WeaponData weaponData2)
        {
            if (weaponData1 is null)
            {
                throw new ArgumentNullException(nameof(weaponData1));
            }

            if (weaponData2 is null)
            {
                throw new ArgumentNullException(nameof(weaponData2));
            }

            this.firstWeaponData = weaponData1;
            this.secondWeaponData = weaponData2;

            this.usingFirstWeaponData = true;
            this.SetWeapon(this.firstWeaponData);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            if (this.weaponBehaviour != null)
            {
                if (Input.GetButtonDown("Slash1"))
                {
                    this.weaponBehaviour.Fire(this.GetComponent<DynamicEntity>());
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    // TODO: Make pretty.
                    this.weaponBehaviour.SwitchWeaponMode();
                    this.weaponBehaviour.Fire(this.GetComponent<DynamicEntity>());
                    this.weaponBehaviour.SwitchWeaponMode();
                }

                if (Input.GetKeyDown(KeyCode.M))
                {
                    // Switch WeaponData being used
                    this.SetWeapon(this.usingFirstWeaponData ? this.firstWeaponData : this.secondWeaponData);
                    this.usingFirstWeaponData = !this.usingFirstWeaponData;
                    Debug.Log($"usingFirstWeaponData: {this.usingFirstWeaponData}");
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
            if (this.weaponBehaviour != null)
            {
                this.dormantWeapons.Add(this.weaponBehaviour);
                this.weaponBehaviour.enabled = false;
            }

            this.weaponBehaviour = null;
        }
    }
}