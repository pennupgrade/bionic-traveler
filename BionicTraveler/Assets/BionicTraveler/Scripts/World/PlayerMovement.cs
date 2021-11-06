namespace BionicTraveler.Scripts.World
{
    using UnityEngine;

    /// <summary>
    /// Describes the player's current movement state.
    /// </summary>
    public enum MovementState
    {
        /// <summary>
        /// Normal movement.
        /// </summary>
        Normal,

        /// <summary>
        /// Traversal movement.
        /// </summary>
        Traverse,
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

        private Vector2 movement;
        private PlayerEntity player;

        /// <summary>
        /// Gets the current movement state.
        /// </summary>
        public MovementState CurrentMovementState => this.moveState;

        // Start is called before the first frame update
        private void Start()
        {
            this.player = this.gameObject.GetComponent<PlayerEntity>();
            this.moveState = MovementState.Normal;
        }

        // Update is called once per frame
        private void Update()
        {
            if (this.player.IsStunned)
            {
                return;
            }

            // Reset state.
            this.moveState = MovementState.Normal;

            if (this.movement != Vector2.zero)
            {
                //this.animator.SetFloat("LastHorizontal", this.movement.x);
                //this.animator.SetFloat("LastVertical", this.movement.y);
            }

            this.movement.x = Input.GetAxisRaw("Horizontal");
            this.movement.y = Input.GetAxisRaw("Vertical");

            //this.animator.SetFloat("Horizontal", this.movement.x);
            //this.animator.SetFloat("Vertical", this.movement.y);
            //this.animator.SetFloat("Speed", this.movement.sqrMagnitude);

            // Note that due to how the blend tree is set up, idle jump will never be transitioned into from movement, so we can update
            // jump regardless (might be cleaner to check for idle prior to updating it in code too?). 
            var isJumping = Input.GetButtonDown("Jump");
            var isIdle = this.movement == Vector2.zero;
            if (isJumping)
            {
                if (isIdle)
                {
                    // Reset direction to down. This is our player idle jump.
                    //this.animator.SetFloat("LastHorizontal", 0);
                    //this.animator.SetFloat("LastVertical", -1);
                }
                else
                {
                    // Dashing!
                    this.movementSpeedFrameMult = 7.5f;
                }
            }

            //this.animator.SetBool("IsJumping", isJumping);
        }

        private void FixedUpdate()
        {
            var currentSpeed = this.movementSpeed;
            var currentMovement = this.movement;
            //var isJumping = this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump");

            this.rb.MovePosition(this.rb.position + (currentMovement * currentSpeed *
                this.movementSpeedFrameMult * Time.fixedDeltaTime));

            // Reset once per frame settings.
            this.movementSpeedFrameMult = 1.0f;
        }
    }
}