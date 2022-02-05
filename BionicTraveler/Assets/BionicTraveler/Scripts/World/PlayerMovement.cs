﻿namespace BionicTraveler.Scripts.World
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
        private float movementSpeed = 3f;
        [SerializeField]
        private AudioClip dashSound;
        private PlayerEntity player;

        [SerializeField]
        [Tooltip("How long the dash is on cooldown before being usable again")]
        private float dashCooldown = 5;
        private GameTime lastDash;

        private TaskPlayerMovement mainMovementTask;
        private TaskAnimated specialMovementTask;

        public void ResetAnimationState()
        {
            this.StopAllMovement("ResetAnimationState");
            this.moveState = MovementState.Default;
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
            if (this.player.IsStunned || this.player.IsBeingKnockedBack)
            {
                this.StopAllMovement("Stunned or being knocked back");
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

            // TODO: Manage cooldowns - should be managed elsewhere, though.
            if (Input.GetButtonDown("Slash1"))
            {
                this.StopAllMovement("About to attack!");
                var combatBehavior = this.player.GetComponent<CombatBehaviour>();
                this.specialMovementTask = new TaskAttack(this.player, combatBehavior.weaponBehaviour, true);
                this.specialMovementTask.Assign();
            }

            if (this.moveState == MovementState.Default)
            {
                if (this.mainMovementTask == null || this.mainMovementTask.HasEnded)
                {
                    this.StopAllMovement("Default State");
                    this.mainMovementTask = new TaskPlayerMovement(this.player, this.movementSpeed);
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