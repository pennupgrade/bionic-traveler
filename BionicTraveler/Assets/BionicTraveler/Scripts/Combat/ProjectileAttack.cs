namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes a ranged attack that creates a projectile.
    /// </summary>
    public class ProjectileAttack : Attack
    {
        private bool hasImpacted;
        private Entity hitEntity;
        private bool hasCollidedWithOwner;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            // Hit entity is read from collision event.
            return this.hitEntity != null ? new Entity[] { this.hitEntity } : new Entity[0];
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            if (targets.Length > 0)
            {
                foreach (var target in targets)
                {
                    target.OnHit(this);
                }

                this.hasImpacted = true;
            }
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasImpacted;
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
    }
}
