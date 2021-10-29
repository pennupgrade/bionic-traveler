namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Manages a weapon's behavior with support for starting attacks and switching attack modes.
    /// </summary>
    public class WeaponBehavior : MonoBehaviour
    {
        private bool isUsingPrimaryAttack;
        private GameTime lastPrimaryAttackTime;
        private GameTime lastSecondaryAttackTime;

        /// <summary>
        /// Gets the weapon data used.
        /// </summary>
        public WeaponData WeaponData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all attacks owned by this object have been disposed.
        /// </summary>
        public bool HaveAllAttacksBeenDisposed =>
                this.gameObject.GetComponents<Attack>().All(attack => attack.HasBeenDisposed);

        /// <summary>
        /// Sets the weapon data. Should only be used once. Since Unity does not allow constructors
        /// for GameObjects we have to make do with this.
        /// </summary>
        /// <param name="weaponData">The weapon data.</param>
        public void SetData(WeaponData weaponData)
        {
            if (this.WeaponData != null)
            {
                throw new InvalidOperationException($"{nameof(this.WeaponData)} is readonly after being set.");
            }

            this.WeaponData = weaponData ?? throw new ArgumentNullException(nameof(weaponData));
        }

        /// <summary>
        /// Fires the weapon.
        /// </summary>
        /// <param name="owner">The entity owning the attack.</param>
        public void Fire(DynamicEntity owner)
        {
            // Ensure we are at the position of our owner, so our attack spawns right where we are.
            this.transform.position = owner.transform.position;

            var attackData = this.GetAttackData();
            if (attackData == null)
            {
                Debug.LogWarning($"WeaponBehavior::Update: No weapon data specified for " +
                    $"{this.WeaponData.Id}, primary: {this.isUsingPrimaryAttack}");
            }
            else
            {
                if (this.GetLastAttackTime().HasTimeElapsed(attackData.Cooldown))
                {
                    Debug.Log("WeaponBehavior::Update: Starting new attack!");
                    var attack = AttackFactory.CreateAttack(this.gameObject, this.WeaponData.PrimaryAttackData);
                    attack.StartAttack();
                    this.SetLastAttackTime(GameTime.Now);
                }
            }
        }

        /// <summary>
        /// Changes the attack mode of the weapon.
        /// </summary>
        public void SwitchWeaponMode()
        {
            if (this.WeaponData.SecondaryAttackData != null)
            {
                this.SwitchAttackMode();
            }
            else
            {
                Debug.LogWarning("WeaponBehavior::Update: No secondary attack mode specified");
            }
        }

        private void Start()
        {
            if (this.WeaponData == null)
            {
                throw new InvalidOperationException($"Weapon behavior requires {nameof(this.WeaponData)} " +
                    $"to be specified. Call {nameof(this.SetData)} after behavior creation.");
            }

            this.isUsingPrimaryAttack = true;
            this.lastPrimaryAttackTime = GameTime.Default;
            this.lastSecondaryAttackTime = GameTime.Default;
        }

        private void OnDestroy()
        {
            foreach (var attack in this.gameObject.GetComponents<Attack>())
            {
                Destroy(attack);
            }

            Debug.Log("WeaponBehavior::OnDestroy: Cleaned up all attacks");
        }

        private void SwitchAttackMode()
        {
            this.isUsingPrimaryAttack = !this.isUsingPrimaryAttack;
            Debug.Log("WeaponBehavior::Update: Switched weapon attack mode");
        }

        private AttackData GetAttackData() => this.isUsingPrimaryAttack
            ? this.WeaponData.PrimaryAttackData : this.WeaponData.SecondaryAttackData;

        private GameTime GetLastAttackTime() => this.isUsingPrimaryAttack
            ? this.lastPrimaryAttackTime : this.lastSecondaryAttackTime;

        private void SetLastAttackTime(GameTime time)
        {
            // This allows us to ensure quickly switching weapons does not reset the timer.
            if (this.isUsingPrimaryAttack)
            {
                this.lastPrimaryAttackTime = time;
            }
            else
            {
                this.lastSecondaryAttackTime = time;
            }
        }
    }
}