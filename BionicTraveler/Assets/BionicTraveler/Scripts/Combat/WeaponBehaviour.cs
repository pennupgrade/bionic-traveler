namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Manages a weapon's behaviour with support for starting attacks and switching attack modes.
    /// </summary>
    public class WeaponBehaviour : MonoBehaviour
    {
        private bool isUsingPrimaryAttack;
        private GameTime lastPrimaryAttackTime;
        private GameTime lastSecondaryAttackTime;
        private DynamicEntity owner;
        private Attack lastAttack;
        private bool isAttacking;
        private int initialSortingLayer;

        /// <summary>
        /// Gets the weapon data used.
        /// </summary>
        public WeaponData WeaponData { get; private set; }

        /// <summary>
        /// Gets the world object.
        /// </summary>
        public GameObject WorldObject { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this weapon is unarmed (no weapon).
        /// </summary>
        public bool IsUnarmed => this.WeaponData.Id == "weapon_unarmed";

        /// <summary>
        /// Gets a value indicating whether all attacks owned by this object have been disposed.
        /// TODO: Fix for prefabs.
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

            this.isUsingPrimaryAttack = true;
            this.lastPrimaryAttackTime = GameTime.Default;
            this.lastSecondaryAttackTime = GameTime.Default;
        }

        /// <summary>
        /// Sets the world object representing this behaviour.
        /// </summary>
        /// <param name="gameObject">The world object.</param>
        /// <param name="owner">The entity owning the world object.</param>
        public void SetWorldObject(GameObject gameObject, DynamicEntity owner)
        {
            this.WorldObject = gameObject;
            this.owner = owner;
            this.SetToIdle();

            var spriteRenderer = this.WorldObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                this.initialSortingLayer = spriteRenderer.sortingOrder;
            }
        }

        /// <summary>
        /// Fires the weapon.
        /// </summary>
        /// <param name="owner">The entity owning the attack.</param>
        public void Fire(DynamicEntity owner)
        {
            this.owner = owner;
            this.isAttacking = true;

            var attackData = this.GetAttackData();
            if (attackData == null)
            {
                Debug.LogWarning($"WeaponBehavior::Fire: No weapon data specified for " +
                    $"{this.WeaponData.Id}, primary: {this.isUsingPrimaryAttack}");
            }
            else
            {
                if (this.IsReady(owner))
                {
                    Debug.Log($"WeaponBehavior::Update: Starting new attack {attackData.DisplayName}!");
                    this.lastAttack = AttackFactory.CreateAttack(this.gameObject, attackData);
                    this.lastAttack.StartAttack(owner);
                    this.SetLastAttackTime(GameTime.Now);
                }
            }
        }

        /// <summary>
        /// Checks whether the current attack can be executed based on <see cref="owner"/>.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>Whether the attack can be executed.</returns>
        public bool IsReady(DynamicEntity owner)
        {
            var attackData = this.GetAttackData();
            if (attackData == null)
            {
                Debug.LogWarning($"WeaponBehavior::IsReady: No weapon data specified for " +
                    $"{this.WeaponData.Id}, primary: {this.isUsingPrimaryAttack}");
                return false;
            }

            bool hasCooldownElapsed = this.GetLastAttackTime().HasTimeElapsed(attackData.Cooldown);
            bool hasEnoughEnergy = attackData.Cost == 0 || owner.Energy >= attackData.Cost;
            bool hasLastAttackFinishedFiring = this.lastAttack == null
                || (this.lastAttack.HasBeenDisposed || this.lastAttack.HasFinishedFiring);
            return hasLastAttackFinishedFiring && hasCooldownElapsed && hasEnoughEnergy;
        }

        /// <summary>
        /// Marks the attack as stopped and resets the weapon to its idle state.
        /// </summary>
        public void StoppedAttack()
        {
            this.lastAttack?.StopAttack();
            this.isAttacking = false;
            this.SetToIdle();
        }

        /// <summary>
        /// Sets the attack mode of the weapon indicating the attack data to use.
        /// </summary>
        /// <param name="primary">Whether the primrary or secondary attack data should be used.</param>
        public void SetWeaponMode(bool primary)
        {
            if (!primary && this.WeaponData.SecondaryAttackData == null)
            {
                Debug.LogWarning("WeaponBehavior::Update: No secondary attack mode specified");
                this.isUsingPrimaryAttack = true;
            }
            else
            {
                this.isUsingPrimaryAttack = primary;
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

        /// <summary>
        /// Gets the next attack data being used.
        /// </summary>
        /// <returns>The attack data.</returns>
        public AttackData GetNextAttackData() => this.GetAttackData();

        private void Start()
        {
            if (this.WeaponData == null)
            {
                throw new InvalidOperationException($"Weapon behavior requires {nameof(this.WeaponData)} " +
                    $"to be specified. Call {nameof(this.SetData)} after behavior creation.");
            }
        }

        private void Update()
        {
            if (this.owner != null && this.WorldObject != null && !this.isAttacking && !this.IsUnarmed)
            {
                var offset = this.WeaponData.AttachOffset;
                if (this.owner.Direction == Vector3.right)
                {
                    offset.x *= -1;
                }
                else if (this.owner.Direction == Vector3.down)
                {
                    // Quick fix when walking down, make sword overlay player.
                    this.WorldObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
                }
                else if (this.owner.Direction == Vector3.up)
                {
                    this.WorldObject.GetComponent<SpriteRenderer>().sortingOrder = this.initialSortingLayer;
                }

                this.WorldObject.transform.localPosition = offset;
                this.SetToIdle();
            }
        }

        private void SetToIdle()
        {
            // Reset weapon to its idle animation - usually just one static frame.
            var animator = this.GetComponent<Animator>();
            if (animator != null)
            {
                if (this.owner.Direction == Vector3.right)
                {
                    animator.Play("Idle");
                }
                else
                {
                    if (animator.HasAnimation("IdleLeft"))
                    {
                        animator.Play("IdleLeft");
                    }
                    else
                    {
                        animator.Play("Idle");
                    }
                }
            }
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
