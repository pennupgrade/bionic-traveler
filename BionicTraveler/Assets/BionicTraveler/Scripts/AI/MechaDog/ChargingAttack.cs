namespace BionicTraveler.Scripts.AI.MechaDog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class ChargingAttack : Attack
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
            // Manually trace collisions as our attack moves quite fast and might skip colliders otherwise.
            var colls = new List<Collider2D>();
            Physics2D.GetContacts(this.GetComponent<Collider2D>(), colls);
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerEntity>().Kill();
            }
        }
    }
}
