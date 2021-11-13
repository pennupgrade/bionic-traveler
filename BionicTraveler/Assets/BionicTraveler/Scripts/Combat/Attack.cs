namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Abstract class for an attack, containing few defined properties and functions,
    /// other specific behavior is located in concrete attack implementations.
    /// </summary>
    public abstract class Attack : MonoBehaviour
    {
        private bool isRunning;

        /// <summary>
        /// Gets the associated attack data.
        /// </summary>
        public AttackData AttackData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all resources associated with this attack have been disposed.
        /// </summary>
        public bool HasBeenDisposed { get; private set; }

        /// <summary>
        /// Gets the owner of the attack.
        /// </summary>
        protected DynamicEntity Owner { get; private set; }

        /// <summary>
        /// Sets the attack data. Should only be used once. Since Unity does not allow constructors
        /// for GameObjects we have to make do with this.
        /// </summary>
        /// <param name="attackData">The attack data.</param>
        public void SetData(AttackData attackData)
        {
            if (this.AttackData != null)
            {
                throw new InvalidOperationException($"{nameof(this.AttackData)} is readonly after being set.");
            }

            this.AttackData = attackData ?? throw new ArgumentNullException(nameof(attackData));
        }

        /// <summary>
        /// Called when the attack was just started. By default, moves the attack to the owner's position.
        /// </summary>
        public virtual void OnAttackStarted()
        {
            this.transform.position = this.Owner.transform.position;
        }

        /// <summary>
        /// Gets one or more targets for the attack.
        /// </summary>
        /// <returns>The target(s) or an empty array. Must not return null.</returns>
        public abstract Entity[] GetTargets();

        /// <summary>
        /// Checks whether an entity is a valid target for this attack.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>A value indicating whether the entity is a valid target.</returns>
        public virtual bool IsValidTarget(Entity target)
        {
            return true;
        }

        /// <summary>
        /// Executes the attack against one or more targets.
        /// </summary>
        /// <param name="targets">The target(s) of the attack.</param>
        public abstract void AttackTargets(Entity[] targets);

        /// <summary>
        /// Returns whether this attack has finished and can be cleaned up.
        /// </summary>
        /// <returns>A value indicating whether the attack has finished.</returns>
        public abstract bool HasFinished();

        /// <summary>
        /// Cleans up any remaining attack resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        ///  /// Starts the attack.
        /// First calls <see cref="GetTargets"/> to get nearby targets.
        /// Then calls <see cref="IsValidTarget(Entity)"/> on all targets to determine whether they can be attacked.
        /// Lastly, calls <see cref="AttackTargets(Entity[])"/> to execute the attack.
        /// Continues every tick until attack has finished.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void StartAttack(DynamicEntity owner)
        {
            this.Owner = owner;
            this.isRunning = true;
            this.OnAttackStarted();
            this.ExecuteAttack();
        }

        private void ExecuteAttack()
        {
            Debug.Log("Attack::ExecuteAttack: Getting targets");
            var targets = this.GetTargets();
            if (targets == null)
            {
                throw new InvalidOperationException($"{nameof(this.GetTargets)} must not return null.");
            }

            Debug.Log($"Attack::ExecuteAttack: We found {targets.Length} targets nearby");

            var validTargets = new List<Entity>();
            foreach (var target in targets)
            {
                if (this.IsValidTarget(target))
                {
                    validTargets.Add(target);
                }
            }

            Debug.Log($"Attack::ExecuteAttack: {validTargets.Count} are valid targets");
            this.AttackTargets(validTargets.ToArray());
            this.PlayAttackAudio();
            Debug.Log($"Attack::ExecuteAttack: Attacked all target(s)");
            if (this.HasFinished())
            {
                Debug.Log($"Attack::ExecuteAttack: Attack has finished");
                this.Dispose();
                this.isRunning = false;
                this.HasBeenDisposed = true;

                if (this.AttackData.Prefab != null)
                {
                    Destroy(this.gameObject);
                    Debug.Log($"Attack::ExecuteAttack: Freed prefab object");
                }

                Destroy(this);
            }
        }

        private void PlayAttackAudio()
        {
            if (this.AttackData.AudioClip != null)
            {
                AudioManager.Instance.PlayOneShot(this.AttackData.AudioClip);
                Debug.Log("aud");
            }
        }

        private void Update()
        {
            if (this.isRunning)
            {
                this.ExecuteAttack();
            }
        }
    }
}
