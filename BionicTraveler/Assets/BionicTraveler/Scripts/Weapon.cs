namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        //stats
        public int damage {get; set;}
        /*1 -> Projectile
        //2 -> LightMelee
        //3 -> HeavyMelee
        4 -> Defense*/
        
        public List <int> attackTypes {get; set;}
        //add item

        
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }
}
