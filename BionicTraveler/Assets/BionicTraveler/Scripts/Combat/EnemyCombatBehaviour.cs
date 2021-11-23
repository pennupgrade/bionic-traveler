namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Manages enemy combat behavior of the currently equipped weapon.
    /// </summary>
    public class EnemyCombatBehaviour : MonoBehaviour
    {
        private WeaponBehaviour weaponBehaviour;
        private List<WeaponBehaviour> dormantWeapons;

        [SerializeField]
        [Tooltip("The weapon data to use.")]
        private WeaponData firstWeaponData;

        [SerializeField]
        [Tooltip("The second weapon data to use.")]
        private WeaponData secondWeaponData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyCombatBehaviour"/> class.
        /// </summary>
        public EnemyCombatBehaviour()
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
        private void InitWeapons(WeaponData weaponData1, WeaponData weaponData2)
        {
            if (weaponData1 is null)
            {
                throw new ArgumentNullException(nameof(weaponData1));
            }

            this.firstWeaponData = weaponData1;
            this.secondWeaponData = weaponData2;
            this.SetWeapon(this.firstWeaponData);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            if (this.weaponBehaviour != null)
            {
                var enemies = this.GetComponent<DynamicEntity>().EnemyScanner.GetEnemies();
                var sortedEnemies = enemies.OrderBy(enemy => Vector3.Distance(enemy.transform.position, this.transform.position));

                // TODO: Ideally only attack the entity we are following.
                GameObject closestEnemy = sortedEnemies.FirstOrDefault();
                if (closestEnemy != null)
                {
                    var playerPos = closestEnemy.transform.position;
                    var ourPos = this.transform.position;
                    var diff = Vector3.Distance(playerPos, ourPos);

                    // TODO: Select best weapon and attack mode based on range.
                    if (diff < this.firstWeaponData.PrimaryAttackData.Range)
                    {
                        this.weaponBehaviour.Fire(this.GetComponent<DynamicEntity>());
                    }
                    else if (this.secondWeaponData != null && diff < this.secondWeaponData.SecondaryAttackData.Range)
                    {
                        this.weaponBehaviour.SwitchWeaponMode();
                        this.weaponBehaviour.Fire(this.GetComponent<DynamicEntity>());
                        this.weaponBehaviour.SwitchWeaponMode();
                    }
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
