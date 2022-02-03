namespace BionicTraveler.Scripts.World
{
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Audio;
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
        private AudioClip dashSound;
        private PlayerEntity player;

        [SerializeField]
        [Tooltip("How long the dash is on cooldown before being usable again")]
        private float dashCooldown = 5;

        private bool dashAvailable = true;

        private TaskPlayerMovement mainMovementTask;
        private TaskAnimated specialMovementTask;

        // Start is called before the first frame update
        private void Start()
        {
            this.player = this.gameObject.GetComponent<PlayerEntity>();
            this.moveState = MovementState.Default;
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

            if (Input.GetButtonDown("Dash"))
            {
                this.StopAllMovement("About to dash!");
                this.specialMovementTask = new TaskDash(this.player);
                this.specialMovementTask.Assign();
                this.moveState = MovementState.Dashing;
            }

            if (this.moveState == MovementState.Default)
            {
                // TODO: Abort whatever other task.
                if (this.mainMovementTask == null || this.mainMovementTask.HasEnded)
                {
                    this.mainMovementTask = new TaskPlayerMovement(this.player, this.movementSpeed);
                    this.mainMovementTask.Assign();
                }
            }
            else if (this.moveState == MovementState.Dashing)
            {
                if (this.specialMovementTask.HasEnded)
                {
                    this.moveState = MovementState.Default;
                }
            }

            //if (Input.GetButtonDown("Dash"))
            //{
            //    if (dashAvailable)
            //    {
            //        // Dashing!
            //        this.movementSpeedFrameMult = 15f;
            //        AudioManager.Instance.PlayOneShot(this.dashSound);
            //        this.StartCoroutine(this.DashController(this.dashCooldown));
            //    }
            //    else
            //    {
            //        Debug.Log($"Dash has a {this.dashCooldown} second cooldown!");
            //    }
            //}
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