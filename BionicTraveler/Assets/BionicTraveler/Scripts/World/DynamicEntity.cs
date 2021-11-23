namespace BionicTraveler.Scripts.World
{
    using System.Collections;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// Class for all entities that can move themselves.
    /// </summary>
    public class DynamicEntity : Entity
    {
        private Vector3 velocity;
        private bool stunned;

        [SerializeField]
        [TooltipAttribute("The items to drop.")]
        private LootTable loot;

        [SerializeField]
        [TooltipAttribute("The entity relationships.")]
        private EntityRelationships relationships;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity"/> class.
        /// </summary>
        public DynamicEntity()
        {
            this.Inventory = new Inventory(this);
        }

        /// <summary>
        /// Gets the inventory.
        /// </summary>
        public Inventory Inventory { get; }

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
        internal Vector3 Velocity { get => this.velocity; set => this.velocity = value; }

        /// <summary>
        /// Gets or sets a value indicating whether entity is stunned.
        /// </summary>
        internal bool IsStunned { get => this.stunned; set => this.stunned = value; }

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

        private void Awake()
        {
            if (this.loot != null)
            {
                foreach (var item in this.loot.Items)
                {
                    this.Inventory.AddItem(item);
                }
            }
        }

        /// <summary>
        /// Start is called just before any of the Update methods is called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            this.EnemyScanner = new EnemyScanner(this.relationships);
        }

        /// <summary>
        /// Function for moving a dynamic entity to a target position.
        /// </summary>
        /// <param name="target">Target world position to move to.</param>
        public void MoveTo(Vector3 target, bool smooth = false)
        {
            this.SetDirection(target);

            base.MoveTo(target, smooth);

        }

        /// <summary>
        /// Makes DEntity invincible for specified number of milliseconds
        /// </summary>
        /// <param name="ms">Number of milliseconds to remain invincible</param>
        public void IFrame(int ms)
        {
            this.StartCoroutine(this.IFrameHandler(ms));
        }

        /// <summary>
        /// Stagger/Stun the entity for the specified number of milliseconds
        /// </summary>
        /// <param name="ms">The number of milliseconds to stun the entity</param>
        public void Stagger(int ms)
        {
            this.StartCoroutine(this.StaggerHandler(ms));
        }

        private IEnumerator IFrameHandler(int ms)
        {
            this.IsInvincible = true;
            yield return new WaitForSeconds(ms / 1000f);
            this.IsInvincible = false;
        }

        private IEnumerator StaggerHandler(int ms)
        {
            this.IsStunned = true;
            yield return new WaitForSeconds(ms / 1000f);
            this.IsStunned = false;
        }

        /// <inheritdoc/>
        public override void OnDied()
        {
            var item = this.Inventory.DropAll();
            base.OnDied();
        }
    }
}