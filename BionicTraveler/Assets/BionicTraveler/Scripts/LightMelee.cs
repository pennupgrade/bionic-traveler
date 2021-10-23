namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// LightMelee implementation for Attack
    /// </summary>
    public class LightMelee : IAttack<LightMelee>
    {
        // properties
        public float range { get; set; }
        public float cooldown { get; set; }
        public float freezeDuration { get; set; }

        private float fovAngle;

        // functions
        public void executeAttack(LightMelee t)
        {
            throw new NotImplementedException();
        }

        public bool didHit(LightMelee t)
        {
            throw new NotImplementedException();
        }
    }
}
