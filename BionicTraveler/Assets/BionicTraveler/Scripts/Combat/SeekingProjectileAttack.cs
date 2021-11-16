﻿namespace BionicTraveler.Scripts.Combat
{
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Describes a ranged attack that creates a projectile.
    /// </summary>
    public class SeekingProjectileAttack : Attack
    {
        private bool hasImpacted;
        private bool isOutOfRange;
        private Entity hitEntity;
        private bool hasCollidedWithOwner;
        private DynamicEntity target;
        private Vector3 startDirection;
        private Vector3 startPosition;
        private Vector3 currentDirection;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            // Hit entity is read from collision event.
            return this.hitEntity != null ? new Entity[] { this.hitEntity } : new Entity[0];
        }

        /// <inheritdoc/>
        public override void OnAttackStarted()
        {
            // Put attack at player position when we start.
            base.OnAttackStarted();

            var enemies = GameObject.FindGameObjectsWithTag("Enemy").Where(
              obj => obj.GetComponent<DynamicEntity>() != null);
            var closestEnemy = enemies.OrderBy(enemy => enemy.transform.DistanceTo(this.transform)).FirstOrDefault();

            this.target = closestEnemy?.GetComponent<DynamicEntity>();
            this.startDirection = this.Owner.Direction;
            this.startPosition = this.transform.position;
            this.currentDirection = this.startDirection;
        }

        /// <inheritdoc/>
        public override bool ShouldPlayImpactAudio()
        {
            return !this.isOutOfRange;
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            if (targets.Length > 0)
            {
                foreach (var target in targets)
                {
                    // TODO: Make friendlies react to being hit.
                    if (target.tag == "Enemy")
                    {
                        target.OnHit(this);
                    }
                }

                this.hasImpacted = true;
            }
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasImpacted || this.isOutOfRange;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            // TODO: Dispose projectile.
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Do not collide with owner of projectile unless we have collided with them before.
            // This ensures that projectiles only collide with the owner once they have left the
            // owners collider once.
            var entity = collision.GetComponent<Entity>();
            if (entity == this.Owner && !this.hasCollidedWithOwner)
            {
                return;
            }

            // We hit something that is not an entity, just remove us.
            if (entity == null)
            {
                this.hasImpacted = true;
                return;
            }

            this.hitEntity = entity;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity == this.Owner)
            {
                this.hasCollidedWithOwner = true;
                return;
            }
        }

        private void FixedUpdate()
        {
            var targetDirection = this.startDirection;
            if (this.target != null)
            {
                targetDirection = (this.target.transform.position - this.transform.position).normalized;
            }

            // Interpolate between current direction and target direction to limit abrupt movement.
            targetDirection = Vector3.Lerp(this.currentDirection, targetDirection, Time.deltaTime * (Random.value * 3));
            this.currentDirection = targetDirection;

            this.gameObject.transform.position = this.gameObject.transform.position +
                (targetDirection * this.AttackData.ProjectileSpeed * Time.deltaTime);

            // If we are too far away from our starting position, finish the attack.
            var currentDistance = Vector3.Distance(this.transform.position, this.startPosition);
            if (currentDistance > this.AttackData.Range)
            {
                this.isOutOfRange = true;
            }
        }
    }
}