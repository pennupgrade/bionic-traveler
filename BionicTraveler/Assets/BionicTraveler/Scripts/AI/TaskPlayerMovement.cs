namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Manages player movement for idle and running.
    /// </summary>
    public class TaskPlayerMovement : TaskAnimated
    {
        private PlayerEntity player;
        private Vector2 movement;
        private Rigidbody2D rb;
        private bool isIdle;
        private float movementSpeed;
        private float movementSpeedFrameMult;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskPlayerMovement"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="movementSpeed">The base movement speed.</param>
        public TaskPlayerMovement(PlayerEntity owner, float movementSpeed)
            : base(owner)
        {
            this.player = owner;
            this.rb = this.player.GetComponent<Rigidbody2D>();
            this.movementSpeed = movementSpeed;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.PlayerMovement;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            this.Animator.Play("Idle");
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {
            // Set sane per tick defaults.
            this.movementSpeedFrameMult = 1;

            if (this.movement != Vector2.zero)
            {
                this.player.SetDirection(this.rb.position + this.movement);
                this.isIdle = false;
            }
            else
            {
                this.isIdle = true;
            }

            if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
            {
                this.movementSpeedFrameMult = 5;
                this.isIdle = false;
            }

            this.movement.x = Input.GetAxis("Horizontal");
            this.movement.y = Input.GetAxis("Vertical");

            // Do actual movement.
            this.rb.MovePosition(this.rb.position + (this.movement * this.movementSpeed *
                 this.movementSpeedFrameMult * Time.fixedDeltaTime));

            base.OnProcess();
        }

        public void UpdateSpeed(float newSpeed)
        {
            this.movementSpeed = newSpeed;
        }

        /// <inheritdoc/>
        public override void UpdateAnimator()
        {
            this.Animator.SetFloat("Horizontal", this.player.Direction.x);
            this.Animator.SetFloat("Vertical", this.player.Direction.y);
            this.Animator.SetFloat("Speed", this.movement.sqrMagnitude);
            this.Animator.SetInteger("MovementState", this.isIdle ? 0 : 1);
        }
    }
}