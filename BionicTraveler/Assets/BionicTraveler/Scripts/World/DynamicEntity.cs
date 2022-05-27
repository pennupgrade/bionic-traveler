namespace BionicTraveler.Scripts.World
{
    using System.Collections;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Items;
    using BionicTraveler.Scripts.Quests;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// Class for all entities that can move themselves.
    /// </summary>
    public class DynamicEntity : Entity
    {
        private EntityTaskManager taskManager;

        private Vector3 velocity;
        private bool stunned;

        [SerializeField]
        [TooltipAttribute("The items to drop.")]
        private LootTable loot;

        [SerializeField]
        private string name;

        [SerializeField]
        [TooltipAttribute("The entity relationships.")]
        private EntityRelationships relationships;

        [SerializeField]
        [TooltipAttribute("The default weapon to assign.")]
        private WeaponData defaultWeapon;

        [SerializeField]
        [TooltipAttribute("Whether the entity cannot be knocked back by attacks.")]
        private bool cannotBeKnockedBack;

        [SerializeField]
        [TooltipAttribute("Whether the entity has no distinct directional movement animations and needs no animator update for movement.")]
        private bool dontUpdateAnimatorForMovement;

        private float energy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity"/> class.
        /// </summary>
        public DynamicEntity()
        {
            this.Inventory = new Inventory(this);
            this.taskManager = new EntityTaskManager(this);
            this.WeaponsInventory = new WeaponsInventory(this);
        }

        /// <summary>
        /// Gets the inventory.
        /// </summary>
        public Inventory Inventory { get; }

        /// <summary>
        /// Gets the weapon inventory.
        /// </summary>
        public WeaponsInventory WeaponsInventory { get; private set; }

        /// <summary>
        /// Gets the Enemy Scanner.
        /// </summary>
        public EnemyScanner EnemyScanner { get; private set; }

        /// <summary>
        /// Gets the relationships.
        /// </summary>
        public EntityRelationships Relationships => this.relationships;

        /// <summary>
        /// Gets or sets velocity.
        /// </summary>
        public Vector3 Velocity { get => this.velocity; set => this.velocity = value; }

        /// <summary>
        /// Gets or sets a value indicating whether entity is stunned.
        /// </summary>
        public bool IsStunned { get => this.stunned; set => this.stunned = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity is being knocked back.
        /// </summary>
        public bool IsBeingKnockedBack { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity is ignored by other entities.
        /// For instance, potential enemies will not attack this target.
        /// </summary>
        public bool IsIgnoredByEveryone { get; set; }

        /// <summary>
        /// Gets the energy level of the entity which is used for certain special attacks.
        /// </summary>
        public float Energy => this.energy;

        /// <summary>
        /// Gets the task manager.
        /// </summary>
        public EntityTaskManager TaskManager => this.taskManager;

        /// <summary>
        /// Gets a value indicating whether this entity has no directional movement animations and thus does not require movement animator updates.
        /// </summary>
        public bool DontUpdateAnimatorForMovement => this.dontUpdateAnimatorForMovement;

        /// <summary>
        /// Returns a value indicating whether this entity is ahead of <paramref name="position"/> based on its <see cref="this.Direction"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Whether or not the entity is ahead.</returns>
        public bool IsAheadOf(Vector3 position)
        {
            var dir = (position - this.gameObject.transform.position).normalized;
            return Vector3.Dot(this.Direction, dir) < 0;
        }

        /// <summary>
        /// Called when script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            if (this.defaultWeapon == null)
            {
                this.defaultWeapon = GameConfigManager.CurrentConfig.DefaultUnarmedWeapon;
            }

            if (this.relationships == null)
            {
                this.relationships = GameConfigManager.CurrentConfig.DefaultEntityRelations;
            }

            this.WeaponsInventory.AddWeapon(this.defaultWeapon);

            if (this.loot != null)
            {
                this.Inventory.AddFromLootTable(this.loot);
            }
        }

        /// <summary>
        /// Start is called just before any of the Update methods is called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            this.EnemyScanner = new EnemyScanner(this.relationships);
            this.RestoreEnergy();
        }

        /// <summary>
        /// Applies a force to this dynamic entity.
        /// </summary>
        /// <param name="force">The force to be applied.</param>
        public void ApplyForce(Vector2 force)
        {
            this.GetComponent<Rigidbody2D>().isKinematic = false;
            this.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            this.IsBeingKnockedBack = true;

            var navMesh = this.GetComponent<NavMeshAgent>();
            if (!this.IsPlayer && navMesh != null)
            {
                navMesh.enabled = false;
            }
        }

        /// <summary>
        /// Adds the specified amount of energy.
        /// </summary>
        /// <param name="amount">The amount of energy.</param>
        public void AddEnergy(float amount)
        {
            // TODO: Cap?
            this.energy += amount;
        }

        /// <summary>
        /// Removes the specified amount of energy.
        /// </summary>
        /// <param name="amount">The amount of energy.</param>
        public void RemoveEnergy(float amount)
        {
            this.energy = Mathf.Max(0, this.energy - amount);
            if (this.energy == 0)
            {
                this.OnEnergyDepleted();
            }
        }

        /// <summary>
        /// Sets the energy to the specified amount.
        /// </summary>
        /// <param name="amount">The amount of energy.</param>
        public void SetEnergy(float amount)
        {
            // TODO: Cap.
            this.energy = amount;
        }

        /// <summary>
        /// Restores the energy to the default amount.
        /// </summary>
        public void RestoreEnergy()
        {
            this.energy = 100;
        }

        /// <inheritdoc/>
        public override bool OnHit(Attack attack)
        {
            var hit = base.OnHit(attack);
            if (!hit)
            {
                return false;
            }

            // TODO: Get force data from attack.
            if (attack is MeleeAttack && !this.cannotBeKnockedBack && !attack.AttackData.DisableMeleeKnockback)
            {
                var awayVector = this.GetComponent<Transform>().position - attack.Owner.GetComponent<Transform>().position;
                this.ApplyForce(awayVector.normalized * 30);
            }

            return true;
        }

        /// <inheritdoc/>
        public override void OnDied()
        {
            var item = this.Inventory.DropAll();
            var killQuest = new Quests.QuestEventEnemyKilled(this.name);
            QuestManager.Instance.ProcessEvent(killQuest);

            this.taskManager.ShutDown();
            base.OnDied();
        }

        /// <summary>
        /// Called when this entity's energy levels have reached zero. Does nothing by default.
        /// </summary>
        public virtual void OnEnergyDepleted()
        {
        }

        public virtual void ResetAnimationState()
        {

        }

        /// <inheritdoc/>
        protected override void Update()
        {
            base.Update();

            if (this.IsBeingKnockedBack)
            {
                if (this.GetComponent<Rigidbody2D>().velocity.magnitude < 0.3f)
                {
                    this.IsBeingKnockedBack = false;

                    // Player navmesh is used for cutscenes only, do not reactivate here.
                    var navMesh = this.GetComponent<NavMeshAgent>();
                    if (!this.IsPlayer && navMesh != null)
                    {
                        navMesh.enabled = true;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // TODO: Might at some point be useful to have physics tasks and normal tasks that are ticked
            // from FixedUpdate/Update respectively.
            this.taskManager.Process();
        }
    }
}