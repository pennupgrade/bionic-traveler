namespace BionicTraveler.Scripts.World
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;
    using UnityEngine.UI;
    using static UnityEngine.Rendering.DebugUI;

    /// <summary>
    /// Player Entity class.
    /// </summary>
    public class PlayerEntity : DynamicEntity
    {
        private PlayerMovement movement;
        /// <summary>
        /// Gets or sets the Player interaction range.
        /// </summary>
        public float InteractionRange { get; set; } = 1;

        private bool diedFromLowEnergy;

        [SerializeField]
        private GameObject blinded;

        /// <summary>
        /// Gets the key manager.
        /// </summary>
        public KeyManager KeyManager { get; private set; }

        /// <summary>
        /// Gets the player movement component.
        /// </summary>
        public PlayerMovement Movement => this.movement;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEntity"/> class.
        /// </summary>
        public PlayerEntity()
        {
            this.KeyManager = new KeyManager();
            this.Inventory.ItemsChanged += this.Inventory_ItemsChanged;
        }

        /// <inheritdoc/>
        protected override void Awake()
        {
            base.Awake();
            this.movement = this.GetComponent<PlayerMovement>();
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
            this.GetComponent<BodypartBehaviour>().enabled = state;
            this.GetComponent<PlayerInteraction>().enabled = state;
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.O))
            {
                this.Blind();
            }
            
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

        public override void ResetAnimationState()
        {
            var playerMovement = this.GetComponent<PlayerMovement>();
            playerMovement.ResetAnimationState();
        }

        private void Instance_FinishedLoading()
        {
            LevelLoadingManager.Instance.FinishedLoading -= this.Instance_FinishedLoading;
            this.SetHealth(100);
            this.RestoreEnergy();
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
                spaceship.StartDialogue(this.gameObject);
            }

            this.diedFromLowEnergy = false;
        }

        private void Inventory_ItemsChanged(Inventory sender, InventoryItem item)
        {
            // TODO: Improve lookup/make singleton.
            var questManager = GameObject.FindObjectOfType<Quests.QuestManager>();
            questManager.ProcessEvent(new Quests.QuestEventInventory());
        }

        private void OnDestroy()
        {
            this.Inventory.ItemsChanged -= this.Inventory_ItemsChanged;
        }

        private void ActivateAbility(Bodypart b)
        {
            b.ActivateAbility();
        }

        private GameTime blindStart = null;
        private GameObject blindObject;

        public void Blind()
        {
            if (blindStart == null)
            {
                blindStart = GameTime.Now;
                blindObject = GameObject.Instantiate(blinded);
                blindObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            }
            else if (blindStart.HasTimeElapsed(3f))
            {
                GameObject.Destroy(blindObject);
            }

        }
    }
}