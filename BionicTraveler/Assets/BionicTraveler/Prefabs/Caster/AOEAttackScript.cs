namespace BionicTraveler.Prefabs.Caster
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using BionicTraveler.Assets.Framework;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class AOEAttackScript : Attack
    {
        private bool hasFinished;
        private Entity hitEntity;
        private bool hasCollidedWithOwner;
        private Vector3 startDirection;
        private Vector3 startPosition;

        private GameTime spawnTime;

        [SerializeField]
        private float activeHitboxTime;

        List<Entity> colliding = new List<Entity>();

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
            
            if (this.spawnTime != null && this.spawnTime.HasTimeElapsed(activeHitboxTime))
            {
                this.hasFinished = true;
            }
        }

        public override void Dispose()
        {
            Destroy(this.gameObject);
        }

        public override Entity[] GetTargets()
        {
            return colliding.ToArray();
            //return this.hitEntity != null ? new Entity[] { this.hitEntity } : new Entity[0];
        }

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
                //this.hitEntity = entity;
                Debug.Log("entity should be set to something");
                Debug.Log(colliding.Count);
            }
            
        }

        public override void OnAttackStarted()
        {
            // override default behavior and do nothing
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
        public void Start()
        {
            spawnTime = GameTime.Now;
        }

    }
}
