namespace BionicTraveler.Scripts.Combat
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes an instant melee attack.
    /// </summary>
    public class MeleeAttack : Attack
    {
        private bool hasAttacked;
        private List<Entity> processedEntities;

        /// <inheritdoc/>
        public override void OnAttackStarted()
        {
            base.OnAttackStarted();

            this.processedEntities = new List<Entity>();
        }

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            var ourCollider = this.GetComponent<Collider2D>();
            if (ourCollider == null)
            {
                Debug.LogWarning("MeleeAttack: No collider found on " + this.gameObject.name);
                return new Entity[0];
            }

            // Manually trace collisions as our attack moves quite fast and might skip colliders otherwise.
            // why do we use non trigger colliders?
            var colls = new List<Collider2D>();
            Physics2D.GetContacts(ourCollider, colls);
            var nonTriggerColls = colls.Where(coll => !coll.isTrigger).ToArray();
            var hitEntities = nonTriggerColls.Select(coll => coll.GetComponent<Entity>())
                .Where(entity => entity != null && !this.processedEntities.Contains(entity))
                .Distinct().ToArray();

            this.processedEntities.AddRange(hitEntities);
            return hitEntities;
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            foreach (var target in targets)
            {
                target.OnHit(this);
            }
        }

        /// <inheritdoc/>
        public override void StopAttack()
        {
            this.hasAttacked = true;
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasAttacked;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }
    }
}
