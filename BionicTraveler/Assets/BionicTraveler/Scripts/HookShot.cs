namespace Items
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class HookShot : BodyPart
    {

       
        [SerializeField]
        private float maxDist = 1.0f;

        [SerializeField]
        private float hookSpeed = 1.0f;

        [SerializeField]
        private GameObject projectilePrefab;

        private GameObject hookProjectile = null;

        private Collider2D collide = null;

        /// <summary>
        /// Activates hookshot ability, sending out projectile in direction of target
        /// </summary>
        public override void ActivateAbility ()
        {
            hookProjectile = GameObject.Instantiate(projectilePrefab, Player.transform.position, Quaternion.identity);
            collide = hookProjectile.GetComponent<Collider2D>();
            //Might Cause Error, double-check
            Vector3 target = Player.Direction.normalized * maxDist;

            //instantiate and throw projectilePrefab forward
            //upon collision
                //if hookable –>
                    //if dynamic –> pull to self
                    //else –> pull self to


        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        new public void Start()
        {
            MechanicBP = Mechanic.TraversalMechanic;
            SlotBP = Slot.RightArm;
        }

    }
}
