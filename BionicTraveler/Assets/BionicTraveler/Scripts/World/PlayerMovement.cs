﻿namespace BionicTraveler.Scripts.World
{
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Audio;
    using UnityEngine;

    /// <summary>
    /// Describes the player's current movement state.
    /// </summary>
    public enum MovementState
    {
        /// <summary>
        /// Idle state
        /// </summary>
        Idle = 0,

        /// <summary>
        /// Running state
        /// </summary>
        Running = 1,

        /// <summary>
        /// Dashing state
        /// </summary>
        Dashing = 2,

        /// <summary>
        /// Player slashing.
        /// </summary>
        Slashing = 3,

        /// <summary>
        /// Player being damaged.
        /// </summary>
        Hurt = 4,

        /// <summary>
        /// Attack animation while stationary.
        /// </summary>
        AttackStationary = 5,
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
        private float movementSpeedFrameMult = 1f; // Multiplier per frame.
        [SerializeField]
        private Rigidbody2D rb;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private AudioClip dashSound;

        private Vector2 movement;
        private PlayerEntity player;

        [SerializeField]
        [Tooltip("How long the dash is on cooldown before being usable again")]
        private float dashCooldown = 5;

        private bool dashAvailable = true;

        /// <summary>
        /// Gets the current movement state.
        /// </summary>
        public MovementState CurrentMovementState => this.moveState;

        /// <summary>
        /// Gets the last movement input.
        /// </summary>
        public Vector2 Movement => this.movement;

        // Start is called before the first frame update
        private void Start()
        {
            this.player = this.gameObject.GetComponent<PlayerEntity>();
            this.moveState = MovementState.Idle;

            this.player.Damaged += this.Player_Damaged;
        }

        private void OnDestroy()
        {
            this.player.Damaged -= this.Player_Damaged;
        }

        // Update is called once per frame
        private void Update()
        {
            if (this.player.IsStunned || this.player.IsBeingKnockedBack)
            {
                return;
            }

            //Debug.Log(this.animator.GetCurrentAnimatorStateInfo(0).ToString());
            

            if (Input.GetButtonDown("Dash"))
            {
                if (dashAvailable)
                {
                    // Dashing!
                    this.movementSpeedFrameMult = 15f;
                    AudioManager.Instance.PlayOneShot(this.dashSound);
                    this.StartCoroutine(this.DashController(this.dashCooldown));
                }
                else
                {
                    Debug.Log($"Dash has a {this.dashCooldown} second cooldown!");
                }
            }

            if (this.movement != Vector2.zero)
            {
                this.gameObject.GetComponent<DynamicEntity>()?.SetDirection(this.rb.position + this.movement);
                this.animator.SetFloat("Horizontal", this.player.Direction.x);
                this.animator.SetFloat("Vertical", this.player.Direction.y);
                this.moveState = (this.movementSpeedFrameMult == 1) ? MovementState.Running : MovementState.Dashing;
            }
            else
            {
                this.moveState = MovementState.Idle;
            }

            this.movement.x = Input.GetAxisRaw("Horizontal");
            this.movement.y = Input.GetAxisRaw("Vertical");

            this.animator.SetFloat("Speed", this.movement.sqrMagnitude);

            // Note that due to how the blend tree is set up, idle jump will never be transitioned into from movement, so we can update
            // jump regardless (might be cleaner to check for idle prior to updating it in code too?).
            //var isJumping = Input.GetButtonDown("Jump");
            var isIdle = this.movement == Vector2.zero;
            //if (isJumping)

            this.animator.SetInteger("MovementState", (int)this.moveState);
            //this.animator.SetBool("IsJumping", isJumping);
        }

        private void Player_Damaged(Entity sender, Entity attacker, bool fatal)
        {
            // Do nothing if we are got killed by this hit as the death animation takes over anyway.
            if (fatal)
            {
                return;
            }

            // Transition to hurt animation. TODO: Play animation more immediately and also figure out if
            // it can be aborted.
            this.moveState = MovementState.Hurt;
            this.animator.SetInteger("MovementState", (int)this.moveState);
        }

        private IEnumerator DashController(float seconds)
        {
            this.dashAvailable = false;
            var elapsed = 0f;
            while (elapsed < seconds)
            {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            this.dashAvailable = true;
        }

        private void FixedUpdate()
        {
            var currentSpeed = this.movementSpeed;
            var currentMovement = this.movement;
            //var isJumping = this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump");

            if (!this.player.IsStunned && !this.player.IsBeingKnockedBack)
            {
                this.rb.MovePosition(this.rb.position + (currentMovement * currentSpeed *
                  this.movementSpeedFrameMult * Time.fixedDeltaTime));
            }

            // Reset once per frame settings.
            //this.movementSpeedFrameMult = 1.0f;
            if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
            {
                this.movementSpeedFrameMult = 1f;
            }
        }
    }
}