namespace BionicTraveler.Scripts.World
{
    using System.Linq;
    using BionicTraveler.Scripts.Items;
    using System.Collections.Generic;

    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Interaction;
    using Framework;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Player Entity class.
    /// </summary>
    public class PlayerEntity : DynamicEntity
    {
        private PlayerMovement movement;
        private bool diedFromLowEnergy;

        [SerializeField]
        private GameObject blinded;

        private GameTime blindStart;
        private GameObject blindObject;

        /// <summary>
        /// Gets or sets the Player interaction range.
        /// </summary>
        public float InteractionRange { get; set; } = 1;

        /// <summary>
        /// Gets the key manager.
        /// </summary>
        public KeyManager KeyManager { get; private set; }

        /// <summary>
        /// Gets the player movement component.
        /// </summary>
        public PlayerMovement Movement => this.movement;

        /// <summary>
        /// Gets a value indicating whether the player is currently being controlled.
        /// Returns false if input is disabled.
        /// </summary>
        public bool HasControl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the player is currently in a dialogue.
        /// </summary>
        public bool IsInDialogue { get; set; }

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
            this.HasControl = true;
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
            this.GetComponent<PlayerMovement>().DisableMovement(!state);
            this.GetComponent<BodypartBehaviour>().enabled = state;
            this.GetComponent<PlayerInteraction>().enabled = state;
            this.HasControl = state;
        }

        protected override void Start()
        {
            base.Start();
            SaveManager.Instance.IsSaving += this.Save;
            SaveManager.Instance.IsLoading += this.Load;
        }

        private void Save()
        {

            SaveManager.Instance.Save("PlayerPositionX", this.transform.position.x);
            SaveManager.Instance.Save("PlayerPositionY", this.transform.position.y);
            SaveManager.Instance.Save("PlayerPositionY", this.transform.position.y);
            SaveManager.Instance.Save("PlayerEnergy", this.Energy);
            SaveManager.Instance.Save("PlayerHealth", this.Health);
            SaveManager.Instance.Save("PlayerInventory", this.Inventory.Items);
            SaveManager.Instance.Save("PlayerWeaponData", AssetDatabase.GetAssetPath(this.WeaponsInventory.WeaponData));


            SaveManager.Instance.Save("PlayerKeyData", this.KeyManager.KeyData);

            // TODO: make chips serializable once active chips are implemented
            //SaveManager.Instance.Save("ActiveChips", ActiveChips);

        }

        private void Load()
        {
            
            this.transform.position = new Vector3((float)SaveManager.Instance.Load("PlayerPositionX"), (float)SaveManager.Instance.Load("PlayerPositionY"), 0);
            this.SetHealth((int) SaveManager.Instance.Load("PlayerHealth"));
            this.SetEnergy((float)SaveManager.Instance.Load("PlayerEnergy"));

            this.Inventory.Clear();
            IReadOnlyCollection<InventoryItem> items = (IReadOnlyCollection <InventoryItem>) SaveManager.Instance.Load("PlayerInventory");
            if (items != null)
                foreach (var item in items)
                {
                    for (var i = 0; i < item.Quantity; i++)
                        this.Inventory.AddItem(item.ItemData);
                }
            
            WeaponData weaponData = (WeaponData)AssetDatabase.LoadAssetAtPath((string)SaveManager.Instance.Load("PlayerWeaponData"), typeof(WeaponData));
            this.WeaponsInventory.Destroy();
            this.WeaponsInventory.AddWeapon(weaponData);
            this.KeyManager = new KeyManager((List<KeyData>)SaveManager.Instance.Load("PlayerKeyData"));
            //ActiveChips = (List<Chip>) SaveManager.Instance.Load("ActiveChips");
        }

        public void Destroy()
        {
            base.Destroy();
            // Always free your events unless you want the GC to keep some partially disposed objects
            // around (hint: you never really want that).
            SaveManager.Instance.IsSaving -= this.Save;
            SaveManager.Instance.IsLoading -= this.Load;
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

        /// <inheritdoc/>
        public override void ResetAnimationState()
        {
            var playerMovement = this.GetComponent<PlayerMovement>();
            playerMovement.ResetAnimationState();
        }

        /// <summary>
        /// Blinds the player.
        /// </summary>
        public void Blind()
        {
            if (this.blindStart == null)
            {
                this.blindStart = GameTime.Now;
                this.blindObject = GameObject.Instantiate(this.blinded);
                this.blindObject.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
            }
            else if (this.blindStart.HasTimeElapsed(3f))
            {
                GameObject.Destroy(this.blindObject);
            }
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

        private void Inventory_ItemsChanged(BasicInventory sender, InventoryItem item)
        {
            // TODO: Improve lookup/make singleton.
            var questManager = GameObject.FindObjectOfType<Quests.QuestManager>();
            questManager.ProcessEvent(new Quests.QuestEventInventory());
        }

        private void OnDestroy()
        {
            this.Inventory.ItemsChanged -= this.Inventory_ItemsChanged;
        }
    }
}