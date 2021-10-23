namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Defense implementation for Attack
    /// </summary>
    public class Defense : IAttack<Defense>
    {
        // properties
        public float range { get; set; }
        public float cooldown { get; set; }
        public float freezeDuration { get; set; }

        private float areaOfEffect;
        private float coolDown;
        private float cost;

        // functions
        public void executeAttack(Defense t)
        {
            throw new NotImplementedException();
        }

        public bool didHit(Defense t)
        {
            throw new NotImplementedException();
        }
    }
}
