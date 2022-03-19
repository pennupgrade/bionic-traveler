namespace BionicTraveler.Scripts.World
{
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Combat;
    using UnityEngine;

    /// <summary>
    /// Describes the player's current movement state.
    /// </summary>
    public enum MovementState
    {
        /// <summary>
        /// Default state
        /// </summary>
        Default = 0,

        Attacking = 1,

        /// <summary>
        /// Dashing state
        /// </summary>
        Dashing = 2,

        /// <summary>
        /// Player being damaged.
        /// </summary>
        Hurt = 4,
    }

    /// <summary>
    /// Handles main player movement.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private MovementState moveState;

        [SerializeField]
        private AudioClip dashSound;
        private PlayerEntity player;

        [SerializeField]
        [Tooltip("How long the dash is on cooldown before being usable again")]
        private float dashCooldown = 5;
        private GameTime lastDash;

        private TaskPlayerMovement mainMovementTask;
        private TaskAnimated specialMovementTask;
        private bool isDisabled;

        public void ResetAnimationState()
        {
            this.StopAllMovement("ResetAnimationState");
            this.moveState = MovementState.Default;
        }

        /// <summary>
        /// Forcefully resets the animator to its idle state.
        /// </summary>
        public void ForceIdleAnimation()
        {
            var animator = this.GetComponent<Animator>();
            animator.SetFloat("Horizontal", this.player.Direction.x);
            animator.SetFloat("Vertical", this.player.Direction.y);
            animator.SetFloat("Speed", 0);
            animator.SetInteger("MovementState", 0);
        }

        public void DisableMovement(bool disable)
        {
            this.StopAllMovement("DisableMovement");
            this.ForceIdleAnimation();
            this.isDisabled = disable;
        }

        // Start is called before the first frame update
        private void Start()
        {
            this.player = this.gameObject.GetComponent<PlayerEntity>();
            this.moveState = MovementState.Default;
            this.lastDash = GameTime.Default;
            //this.player.Damaged += this.Player_Damaged;
        }

        private void OnDestroy()
        {
            //this.player.Damaged -= this.Player_Damaged;
        }

        private void StopAllMovement(string reason)
        {
            this.mainMovementTask?.End(reason, false);
            this.mainMovementTask = null;

            this.specialMovementTask?.End(reason, false);
            this.specialMovementTask = null;
        }

        // Update is called once per frame
        private void Update()
        {
            if (this.isDisabled)
            {
                return;
            }

            if (this.player.IsStunned || this.player.IsBeingKnockedBack)
            {
                this.StopAllMovement("Stunned or being knocked back");
                return;
            }

            if (this.player.IsDeadOrDying)
            {
                this.StopAllMovement("Player is dying");
                return;
            }

            if (Input.GetButtonDown("Dash") && this.lastDash.HasTimeElapsedReset(this.dashCooldown))
            {
                this.StopAllMovement("About to dash!");
                this.specialMovementTask = new TaskDash(this.player);
                this.specialMovementTask.Assign();

                // TODO: Make audio part of task? But then were to define the sound?
                // New thought: Have Tunable system for structures. Tunables are ScriptableObjects
                // that can be queried by structures, perhaps via a central TunableManager?
                AudioManager.Instance.PlayOneShot(this.dashSound);
                this.lastDash = GameTime.Now;
                this.moveState = MovementState.Dashing;
            }

            if (Input.GetButtonDown("Slash1") && !this.player.WeaponsInventory.IsUnarmed)
            {
                this.player.WeaponsInventory.DisplayCurrentWeapon();
                if (this.player.WeaponsInventory.equippedWeaponBehavior.IsReady(this.player))
                {
                    this.StopAllMovement("About to attack!");
                    this.specialMovementTask = new TaskAttack(this.player, true);
                    this.specialMovementTask.Assign();
                    this.moveState = MovementState.Attacking;
                }
            }

            // Secondary attack.
            if (Input.GetKeyDown(KeyCode.Q) && !this.player.WeaponsInventory.IsUnarmed)
            {
                this.player.WeaponsInventory.DisplayCurrentWeapon();
                if (this.player.WeaponsInventory.equippedWeaponBehavior.IsReady(this.player))
                {
                    this.StopAllMovement("About to attack!");
                    this.specialMovementTask = new TaskAttack(this.player, false);
                    this.specialMovementTask.Assign();
                    this.moveState = MovementState.Attacking;
                }
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                this.StopAllMovement("Switching weapon!");
                this.player.WeaponsInventory.ToggleWeaponVisibility();
            }

            if (this.moveState == MovementState.Default)
            {
                if (this.mainMovementTask == null || this.mainMovementTask.HasEnded)
                {
                    this.StopAllMovement("Default State");
                    this.mainMovementTask = new TaskPlayerMovement(this.player);
                    this.mainMovementTask.Assign();
                }
            }
            else if (this.moveState == MovementState.Dashing)
            {
                if (this.specialMovementTask.HasEnded)
                {
                    this.moveState = MovementState.Default;
                    this.specialMovementTask = null;
                }
            }
            else if (this.moveState == MovementState.Attacking)
            {
                if (this.specialMovementTask.HasEnded)
                {
                    this.moveState = MovementState.Default;
                    this.specialMovementTask = null;
                }
            }
        }

        //private void Player_Damaged(Entity sender, Entity attacker, bool fatal)
        //{
        //    // Do nothing if we are got killed by this hit as the death animation takes over anyway.
        //    if (fatal)
        //    {
        //        return;
        //    }

        //    // Transition to hurt animation. TODO: Play animation more immediately and also figure out if
        //    // it can be aborted.
        //    this.moveState = MovementState.Hurt;
        //    this.animator.SetInteger("MovementState", (int)this.moveState);
        //}
    }
}