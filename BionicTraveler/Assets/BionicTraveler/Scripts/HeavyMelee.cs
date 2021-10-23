namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// HeavyMelee implementation for Attack
    /// </summary>
    public class HeavyMelee : IAttack<HeavyMelee>
    {
        // properties
        public float range { get; set; }
        public float cooldown { get; set; }
        public float freezeDuration { get; set; }

        private float cost;
        
        // functions
        public void executeAttack(HeavyMelee t)
        {
            throw new NotImplementedException();
        }

        public bool didHit(HeavyMelee t)
        {
            throw new NotImplementedException();
        }
    }
}
