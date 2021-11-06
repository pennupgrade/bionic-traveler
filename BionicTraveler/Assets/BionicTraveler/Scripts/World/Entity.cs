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
        private bool visible = false;
        [SerializeField]
        private float baseSpeed = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether entity is invincible.
        /// </summary>
        public bool IsInvincible { get; set; }

        /// <summary>
        /// Gets the health from the entity.
        /// </summary>
        public int Health => this.health;

        /// <summary>
        /// Add some fixed value to the current health value.
        /// </summary>
        /// <param name = "amt">The amount of health to be added</param>
        public void AddHealth(float amt)
        {
            this.health = (int)Math.Min(this.health + amt, this.maxHealth);
        }

        /// <summary>
        /// Decrease the current Health value by some fixed amount
        /// </summary>
        /// <param name="amt">The amount of health lost</param>
        public void LoseHealth(float amt)
        {
            this.health = (int)Math.Max(this.health - amt, 0f);
            Debug.Log($"{this.gameObject.name}: My health is now {this.health}");
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
        /// Move the entity to some specified destination
        /// </summary>
        /// <param name="dest">The Transform of the destination to move to</param>
        /// <param name="smooth">Whether to use a SmoothStep function for the movement; default false</param>
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
        /// Destroy the EntityGameObject
        /// </summary>
        public void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Hides the entity in the current Scene
        /// </summary>
        public void MakeInvisible()
        {
            UnityEditor.SceneVisibilityManager.instance.Hide(gameObject, true);
        }

        /// <summary>
        /// Shows/Un-hides the entity in the current Scene
        /// </summary>
        public void MakeVisible()
        {
            UnityEditor.SceneVisibilityManager.instance.Show(gameObject, true);
        }

        /// <summary>
        /// Toggles the visibility state of the entity GameObject in the current Scene
        /// </summary>
        public void ToggleVisibility()
        {
            UnityEditor.SceneVisibilityManager.instance.ToggleVisibility(gameObject, true);
        }

        public bool IsVisible()
        {
            return visible;
        }

        // Use this for initialization
        void Start()
        {
            health = maxHealth;
            visible = !UnityEditor.SceneVisibilityManager.instance.IsHidden(gameObject, true);
        }
    }
}