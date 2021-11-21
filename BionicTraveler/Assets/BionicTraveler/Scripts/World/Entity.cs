namespace BionicTraveler.Scripts.World
{
    using System;
    using System.Collections;
    using BionicTraveler.Scripts.Combat;
    using UnityEngine;

    /// <summary>
    /// Describes a base entity in the game world.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        private int maxHealth = 100;
        private int health;
        private bool isVisible;
        [SerializeField]
        private float baseSpeed = 1f;
        private Vector3 direction;


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
            this.health = (int)Math.Max(this.health - amt, 0f);
            Debug.Log($"{this.gameObject.name}: My health is now {this.health}");

            if (this.health == 0)
            {
                this.Kill();
            }
        }

        /// <summary>
        /// OnHit functionality to be implemented in children of Entity.
        /// </summary>
        /// <param name="attack">The attack.</param>
        public virtual void OnHit(Attack attack)
        {
            if (this.IsInvincible)
            {
                return;
            }

            Debug.Log($"{this.gameObject.name} just got hit");
            var damage = attack.AttackData.GetBaseDamage();
            this.LoseHealth(damage);
        }

        /// <summary>
        /// Move the entity to some specified destination.
        /// </summary>
        /// <param name="dest">The Transform of the destination to move to.</param>
        /// <param name="smooth">Whether to use a SmoothStep function for the movement; default false.</param>
        public void MoveTo(Vector3 dest, bool smooth = false)
        {
            StartCoroutine(Movement(dest, smooth));
        }

        private IEnumerator Movement(Vector3 target, bool smooth)
        {
            Transform pos = gameObject.transform;
            float duration = (pos.position - target).magnitude / baseSpeed;
            float elapsed = 0f;
            float t = elapsed / duration;
            if (smooth) { t = t * t * (3f - 2f * t); }

            while (elapsed < duration)
            {
                pos.position = Vector3.Lerp(pos.position, target, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            pos.position = target;

        }

        /// <summary>
        /// Sets the entity's health to 0 and kills it.
        /// </summary>
        public virtual void Kill()
        {
            Debug.Log("Implement proper dying");
            this.health = 0;
            this.Destroy();
        }

        /// <summary>
        /// Destroy the EntityGameObject.
        /// </summary>
        public void Destroy()
        {
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Hides the entity in the current Scene.
        /// </summary>
        public void MakeInvisible()
        {
            UnityEditor.SceneVisibilityManager.instance.Hide(this.gameObject, true);
        }

        /// <summary>
        /// Shows/Un-hides the entity in the current Scene.
        /// </summary>
        public void MakeVisible()
        {
            UnityEditor.SceneVisibilityManager.instance.Show(this.gameObject, true);
        }

        /// <summary>
        /// Toggles the visibility state of the entity GameObject in the current Scene.
        /// </summary>
        public void ToggleVisibility()
        {
            UnityEditor.SceneVisibilityManager.instance.ToggleVisibility(this.gameObject, true);
        }

        /// <summary>
        /// Start is called just before any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            this.health = this.maxHealth;
            this.isVisible = !UnityEditor.SceneVisibilityManager.instance.IsHidden(this.gameObject, true);
        }
    }
}