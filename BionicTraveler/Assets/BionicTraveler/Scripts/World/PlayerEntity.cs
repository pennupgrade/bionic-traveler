namespace BionicTraveler.Scripts.World
{
    using System.Linq;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Player Entity class.
    /// </summary>
    public class PlayerEntity : DynamicEntity
    {
        /// <summary>
        /// Gets or sets the Player interaction range.
        /// </summary>
        public float InteractionRange { get; set; } = 1;

        // TODO: Uncomment these after merging Combat classes
        private CombatBehaviour PrimaryWeapon;
        private CombatBehaviour SecondaryWeapon;
        //private List<Chip> ActiveChips = new List<Chip>();

        private bool diedFromLowEnergy;

        /// <summary>
        /// Gets the key manager.
        /// </summary>
        public KeyManager KeyManager { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEntity"/> class.
        /// </summary>
        public PlayerEntity()
        {
            this.KeyManager = new KeyManager();
        }

        /// <summary>
        /// Disables relevant components ex. movement and attacking.
        /// </summary>
        public void DisableInput() => this.SetInputState(false);

        /// <summary>
        /// Enables relevant components ex. movement and attacking.
        /// </summary>
        public void EnableInput() => this.SetInputState(true);

        private void SetInputState(bool state)
        {
            this.GetComponent<PlayerMovement>().enabled = state;
            this.GetComponent<CombatBehaviour>().enabled = state;
            this.GetComponent<BodypartBehaviour>().enabled = state;
            this.GetComponent<PlayerInteraction>().enabled = state;
        }

        protected override void Update()
        {
            base.Update();
            
            // TODO: This is inefficient in an open world, refine later; Create helper function to find objects of a certain type
            var interactables = GameObject.FindGameObjectsWithTag("Interactable").Where(
                interactable => Vector3.Distance(
                    interactable.transform.position, this.transform.position) < this.InteractionRange).ToArray();

            foreach (var a in interactables)
            {
                a.GetComponent<IInteractable>().OnInteract(this.gameObject);
            }

            //player pickup
            var pickups = GameObject.FindObjectsOfType<Pickup>().Where(pickup => Vector3.Distance(
                pickup.transform.position,
                this.transform.position) < this.InteractionRange).ToArray();

            foreach (var pickup in pickups)
            {
                if (pickup.transform.DistanceTo(pickup.transform) < pickup.PickUpRange)
                {
                    pickup.PickUp(this);
                }
            }

            //press key to show inventory data
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log(this.Inventory.ToString());
            }

            //Drink Health Potion
            if (Input.GetKeyDown(KeyCode.H))
            {
                foreach (var item in Inventory.Items)
                {
                    this.Inventory.Use(item);
                }
            }

            if (Input.GetKeyDown(KeyCode.O) && this.Inventory.Items.Count > 0)
            {
                var item = this.Inventory.DropItem(this.Inventory.Items.First().ItemData);
            }
        }

        /// <inheritdoc/>
        public override void OnEnergyDepleted()
        {
            this.diedFromLowEnergy = true;
            this.Kill();
        }

        /// <inheritdoc/>
        public override void Kill()
        {
            // TODO: Move to something more general, like maybe PlayerRespawnManager?
            // TODO: Properly reset certain properties such as force.
            base.Kill();
            this.IsInvincible = true;
        }

        /// <inheritdoc/>
        public override void OnDied()
        {
            this.IsStunned = false;
            this.IsBeingKnockedBack = false;

            LevelLoadingManager.Instance.FinishedLoading += this.Instance_FinishedLoading;

            if (!this.diedFromLowEnergy)
            {
                LevelLoadingManager.Instance.ReloadCurrentScene();
            }
            else
            {
                // If we died from low energy, we get moved back to our spaceship. If our current
                // scene does not have one, load last scene that does.
                // TODO: This really needs a proper refactor to get the last scene with a spaceship and then move us there.
                var spaceship = GameObject.FindObjectOfType<Spaceship>();
                if (spaceship != null)
                {
                    LevelLoadingManager.Instance.ReloadCurrentScene();
                }
                else
                {
                    Debug.Log("PlayerEntity::OnDied: Died from low energy, but no spaceship on scene. Loading previous scene");
                    LevelLoadingManager.Instance.StartLoadLevel("LandscapeScene");
                }
            }
        }

        private void Instance_FinishedLoading()
        {
            LevelLoadingManager.Instance.FinishedLoading -= this.Instance_FinishedLoading;
            this.SetHealth(100);
            this.IsInvincible = false;

            // Reset animator state.
            var animator = this.GetComponent<Animator>();
            animator.Rebind();
            animator.Update(0f);

            // TODO: This really needs a proper refactor to get the last scene with a spaceship and then move us there.
            if (this.diedFromLowEnergy)
            {
                // Probe current scene. Not fast or pretty, but works.
                var spaceship = GameObject.FindObjectOfType<Spaceship>();
                this.gameObject.transform.position = spaceship.PlayerExitPoint.transform.position;
            }

            this.diedFromLowEnergy = false;
        }

        private void ActivateAbility(Bodypart b)
        {
            b.ActivateAbility();
        }
    }
}