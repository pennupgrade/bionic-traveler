namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Projectile implementation for Attack
    /// </summary>
    public class Projectile : IAttack<Projectile>
    {
        // properties
        public float range { get; set; }
        public float cooldown { get; set; }
        public float freezeDuration { get; set; }

        private float reloadTime;
        private float maxAmmo;
        private int clipSize;
        private float speed;
        private bool shouldVanish;

        // functions
        public void executeAttack(Projectile t)
        {
            throw new NotImplementedException();
        }

        public bool didHit(Projectile t)
        {
            throw new NotImplementedException();
        }
    }
}
