namespace BionicTraveler.Scripts.AI.HackathonT1
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TrapProjectile : MonoBehaviour
    {
        public Vector3 origin;

        public GameObject stickyTrap;

        // Update is called once per frame
        void Update()
        {
            // If bullet wanders off beyond screen, destroy it.
            // TODO: FIND BETTER METHOD TO FIND EDGE OF SCREEN.
            float distanceTraveled = (this.transform.position - origin).magnitude;

            float throwDistanceRand = UnityEngine.Random.Range(5, 10);

            if (distanceTraveled >= throwDistanceRand)
            {
                // Generate Sticky trap object
                Vector3 spawnPos = this.transform.position;

                GameObject stickyTrap = Instantiate(this.stickyTrap, spawnPos, Quaternion.identity) as GameObject;

                Destroy(gameObject);
            }
        }
    }
}
