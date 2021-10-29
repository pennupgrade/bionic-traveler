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
        private float MaxDist = 1.0f;

        [SerializeField]
        private float HookSpeed = 1.0f;

        [SerializeField]
        private GameObject projectilePrefab;

        private GameObject ActiveHook = null;

        private Collider2D collide;

        public override void ActivateAbility ()
        {
            ActiveHook = GameObject.Instantiate(projectilePrefab, Player.transform, Quaternion.identity);
            collide = ActiveHook.GetComponent<Collider2D>();
            //Might Cause Error, double-check
            target = Player.Facing.normalized * MaxDist

        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        new public void Start()
        {
            MechanicBP = Mechanic.TraversalMechanic;
        }

    }
}
