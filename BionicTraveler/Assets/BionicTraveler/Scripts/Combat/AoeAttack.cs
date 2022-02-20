namespace BionicTraveler.Scripts.Combat
{
    using System.Collections.Generic;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class AoeAttack : Attack
    {
        private bool hasFinished;
        private Entity hitEntity;
        private bool hasCollidedWithOwner;
        private Vector3 startDirection;
        private Vector3 startPosition;

        private GameTime spawnTime;

        [SerializeField]
        private float activeHitboxTime;

        private List<Entity> colliding;

        /// <summary>
        /// Initializes a new instance of the <see cref="AoeAttack"/> class.
        /// </summary>
        public AoeAttack()
        {
            this.colliding = new List<Entity>();
        }

        /// <summary>
        /// Sets the target of the attack which will be its origin.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetOrigin(Vector3 position)
        {
            this.transform.position = position;
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            if (targets.Length > 0)
            {
                foreach (var target in targets)
                {
                    // TODO: Make friendlies react to being hit.
                    if (target.tag == "Player")
                    {
                        target.OnHit(this);
                    }
                }

                this.hasFinished = true;
            }

            if (this.spawnTime != null && this.spawnTime.HasTimeElapsed(this.activeHitboxTime))
            {
                this.hasFinished = true;
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            return this.colliding.ToArray();
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasFinished;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Do not collide with owner of projectile unless we have collided with them before.
            // This ensures that projectiles only collide with the owner once they have left the
            // owners collider once.
            var entity = collision.GetComponent<Entity>();

            // We hit something that is not an entity, just remove us.
            if (entity == null)
            {
                Debug.Log("Hit not entity");
                return;
            }
            else
            {
                this.colliding.Add(entity);
            }
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

        private void Start()
        {
            this.spawnTime = GameTime.Now;
            this.colliding = new List<Entity>();
        }
    }
}
