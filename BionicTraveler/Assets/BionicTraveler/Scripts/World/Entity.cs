namespace BionicTraveler.Scripts.World
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Combat;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Describes a base entity in the game world.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField]
        private float baseMovementSpeed = 3f;

        [SerializeField]
        private float walkSpeed = 2f;

        private int maxHealth = 100;
        private int health;
        private bool isVisible;

        private Vector3 direction;
        private GameTime timeDied;
        private bool isDying;
        private DynamicEntity lastAttacker;
        private DynamicEntity killer;

        private List<IMovementModifier> movementModifiers;
        private Collider2D mainCollider;

        /// <summary>
        /// The event handler for a dying entity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="killer">The killer.</param>
        public delegate void EntityDeathEventHandler(Entity sender, Entity killer);

        /// <summary>
        /// The event handler for a damaged entity.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="fatal">Whether the attack was fatal.</param>
        public delegate void EntityDamagedEventHandler(Entity sender, Entity attacker, bool fatal);

        /// <summary>
        /// Called when the entity started dying.
        /// </summary>
        public event EntityDeathEventHandler Dying;

        /// <summary>
        /// Called when the entity has died.
        /// </summary>
        public event EntityDeathEventHandler Died;

        /// <summary>
        /// Called when the entity got damaged.
        /// </summary>
        public event EntityDamagedEventHandler Damaged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            this.IsPlayer = this is PlayerEntity;
            this.IsDynamic = this is DynamicEntity;
            this.movementModifiers = new List<IMovementModifier>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether entity is invincible.
        /// </summary>
        public bool IsInvincible { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity is visible.
        /// </summary>
        public bool IsVisibible => this.isVisible;

        /// <summary>
        /// Gets the health from the entity.
        /// </summary>
        public int Health => this.health;

        /// <summary>
        /// Gets a value indicating whether this entity is currently dying, i.e. its death animation is playing.
        /// </summary>
        public bool IsDying => this.isDying;

        /// <summary>
        /// Gets a value indicating whether this entity has died, i.e. its death animation has finished.
        /// </summary>
        public bool IsDead => this.health == 0 && !this.isDying;

        /// <summary>
        /// Gets a value indicating whether this entity is dead or dying.
        /// </summary>
        public bool IsDeadOrDying => this.IsDying || this.IsDead;

        /// <summary>
        /// Gets a value indicating whether this entity is a player.
        /// </summary>
        public bool IsPlayer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this entity is dynamic.
        /// </summary>
        public bool IsDynamic { get; private set; }

        /// <summary>
        /// Gets the base movement speed of this entity.
        /// </summary>
        public float BaseMovementSpeed => this.baseMovementSpeed;

        /// <summary>
        /// Gets the current movement speed base on <see cref="BaseMovementSpeed"/> and any potential effects.
        /// </summary>
        public float MovementSpeed { get; private set; }

        /// <summary>
        /// Gets the default walk speed for this entity.
        /// </summary>
        public float WalkSpeed => this.walkSpeed;

        /// <summary>
        /// Gets or sets a value indicating whether this entity should skip its death animation.
        /// </summary>
        public bool SkipDeathAnimation { get; set; }

        /// <summary>
        /// Gets or sets the direction for SpriteRenderer/FSM.
        /// </summary>
        internal Vector3 Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.direction = value;
            }
        }

        /// <summary>
        /// Add some fixed value to the current health value.
        /// </summary>
        /// <param name = "amt">The amount of health to be added.</param>
        public void AddHealth(float amt)
        {
            this.health = (int)Math.Min(this.health + amt, this.maxHealth);
        }

        /// <summary>
        /// Sets this entity's health to <paramref name="health"/>.
        /// </summary>
        /// <param name="health">The health.</param>
        public void SetHealth(int health)
        {
            this.health = health;
        }

        /// <summary>
        /// Gets whether this entity is currently facing left (globally speaking), i.e. its <see cref="Direction"/>'s Vector x-component is negative.
        /// </summary>
        /// <returns>Whether entity is facing left.</returns>
        public bool IsFacingLeft()
        {
            return this.direction.x < 0.0f;
        }

        /// <summary>
        /// Sets Direction for this Dynamic Entity.
        /// </summary>
        /// <param name="target">Target world position to look at</param>
        public void SetDirection(Vector3 target)
        {
            Vector3 pos = this.gameObject.transform.position;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(target.y - pos.y, target.x - pos.x);
            if (angle < 0)
            {
                angle += 360;
            }

            if (angle > 315 || angle < 45)
            {
                this.Direction = Vector3.right;
            }
            else if (angle > 45 && angle < 135)
            {
                this.Direction = Vector3.up;
            }
            else if (angle > 135 && angle < 225)
            {
                this.Direction = Vector3.left;
            }
            else if (angle > 225 && angle < 315)
            {
                this.Direction = Vector3.down;
            }
        }

        /// <summary>
        /// Decrease the current Health value by some fixed amount.
        /// </summary>
        /// <param name="amt">The amount of health lost.</param>
        public void LoseHealth(float amt)
        {
            this.LoseHealth(amt, null);
        }

        private void LoseHealth(float amt, DynamicEntity source)
        {
            this.health = (int)Math.Max(this.health - amt, 0f);
            Debug.Log($"{this.gameObject.name}: My health is now {this.health}");
            this.Damaged?.Invoke(this, source, this.health == 0);

            if (this.health == 0)
            {
                this.Kill(source);
            }
        }

        /// <summary>
        /// OnHit functionality to be implemented in children of Entity.
        /// </summary>
        /// <param name="attack">The attack.</param>
        /// <returns>A value indicating whether the hit was processed.</returns>
        public virtual bool OnHit(Attack attack)
        {
            if (this.IsInvincible)
            {
                return false;
            }

            Debug.Log($"{this.gameObject.name} just got hit");
            this.lastAttacker = attack.Owner;
            var damage = attack.AttackData.GetBaseDamage();
            this.LoseHealth(damage, attack.Owner);
            return true;
        }

        /// <summary>
        /// Sets the entity's health to 0 and kills it.
        /// </summary>
        public virtual void Kill()
        {
            this.Kill(null);
        }

        private void Kill(DynamicEntity killer)
        {
            this.health = 0;
            this.timeDied = GameTime.Now;
            this.killer = killer;

            // Disable most of our behaviors. TODO: Should this be done in each behavior instead?
            // TODO: Have entity metadata that specifies whether movement should be disabled while dying or not.
            this.GetComponent<EnemyCombatBehaviour>()?.Disable();
            //this.GetComponent<ContextSteering>()?.Disable();
            //this.GetComponent<NavMeshAgent>()?.Disable();

            var animator = this.GetComponent<Animator>();
            if (!this.SkipDeathAnimation && animator != null)
            {
                this.isDying = true;
                animator.Play("Dying");
                this.Dying?.Invoke(this, killer);
            }
            else
            {
                this.OnDied();
            }
        }

        /// <summary>
        /// Called when the entity has stopped dying and is transitioning to a death state.
        /// </summary>
        public virtual void OnBeforeDied()
        {
            this.isDying = false;
            this.OnDied();
            this.Died?.Invoke(this, this.killer);
        }

        /// <summary>
        /// Called when the entity has died.
        /// </summary>
        public virtual void OnDied()
        {
            // TODO: Some Entity metadata that distinguishes between whether we should disappear or have a corpse.
            this.Destroy();

            //var animator = this.GetComponent<Animator>();
            //animator.Play("Death");
        }

        /// <summary>
        /// This function is called by the animator of an entity. Do not rename it or the reference breaks!
        /// </summary>
        public void OnDyingAnimationFinished()
        {
            // It seems sometimes Unity fires anim events twice, so we block that here.
            if (this.isDying)
            {
                this.OnBeforeDied();
            }
        }

        /// <summary>
        /// Destroy the EntityGameObject.
        /// </summary>
        public void Destroy()
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Start is called just before any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            this.health = this.maxHealth;
            this.mainCollider = this.GetComponents<Collider2D>().FirstOrDefault(collider => !collider.isTrigger);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// Update is called every fixed frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            var stackModifiers = this.movementModifiers.Where(modifier => modifier.CanStack());
            var nonStackModifiers = this.movementModifiers.Where(modifier => !modifier.CanStack()).GroupBy(m => m.GetType());
            var distinctNonStack = nonStackModifiers.Select(g => g.First());
            var modifiers = stackModifiers.ToList();
            modifiers.AddRange(distinctNonStack);

            var speedMult = modifiers.Aggregate(1f, (x, y) => x * y.GetSpeedMultiplier(this));
            this.MovementSpeed = this.baseMovementSpeed * speedMult;
            this.movementModifiers.Clear();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // If we have no main collider, we cannot do anything.
            if (this.mainCollider == null)
            {
                return;
            }

            if (collision.IsTouching(this.mainCollider))
            {
                var movementModifier = collision.GetComponent<IMovementModifier>();
                if (movementModifier != null && !this.movementModifiers.Contains(movementModifier))
                {
                    this.movementModifiers.Add(movementModifier);
                }
            }
        }
    }
}