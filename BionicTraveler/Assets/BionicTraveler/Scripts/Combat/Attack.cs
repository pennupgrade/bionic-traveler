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
        private bool hasAnimationFiredHit;

        /// <summary>
        /// Gets the associated attack data.
        /// </summary>
        public AttackData AttackData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all resources associated with this attack have been disposed.
        /// </summary>
        public bool HasBeenDisposed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this attack has finished its main firing logic.
        /// For instance, a sword attack after its animation has completed. This allows other
        /// attacks to be used again.
        /// You only need to explicitly set this to true if the attack outlives its animation, for instance
        /// a projectile being fired and moving in the game world after the firing animation has finished.
        /// </summary>
        public bool HasFinishedFiring { get; protected set; }

        /// <summary>
        /// Gets the owner of the attack.
        /// </summary>
        public DynamicEntity Owner { get; private set; }

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
        /// Returns whether audio should be played on attack impact.
        /// </summary>
        /// <returns>A value indicating whether audio should be played.</returns>
        public virtual bool ShouldPlayImpactAudio()
        {
            return true;
        }

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
            this.Owner.RemoveEnergy(this.AttackData.Cost);
            this.OnAttackStarted();
            this.PlayAttackAudio();
            this.ExecuteAttack();
        }

        /// <summary>
        /// Stops the attack.
        /// </summary>
        public virtual void StopAttack()
        {
        }

        /// <summary>
        /// This function is called by the animator of an entity when the attack is supposed to hit. 
        /// Do not rename it or the reference breaks!
        /// </summary>
        public void OnAttackAnimationHit()
        {
            Debug.Log("OnAttackAnimationHit:: Called!");
            this.hasAnimationFiredHit = true;
        }

        private void ExecuteAttack()
        {
            // Attack targets if we either do not need to wait for an animation or if the animation
            // has indicated we should hit now.
            if (!this.AttackData.DoesAnimationDetermineHit || this.hasAnimationFiredHit)
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
                Debug.Log($"Attack::ExecuteAttack: Attacked all target(s)");
            }

            if (this.HasFinished())
            {
                Debug.Log($"Attack::ExecuteAttack: Attack has finished");
                this.Dispose();
                this.isRunning = false;
                this.HasBeenDisposed = true;

                // Play impact audio if need be.
                if (this.ShouldPlayImpactAudio())
                {
                    this.PlayImpactAudio();
                }

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
            }
        }

        private void PlayImpactAudio()
        {
            if (this.AttackData.AudioClipImpact != null)
            {
                AudioManager.Instance.PlayOneShot(this.AttackData.AudioClipImpact);
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
