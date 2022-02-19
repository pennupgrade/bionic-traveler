namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Prefabs.Caster;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class AOEAttackIndicatorScript : MonoBehaviour
    {
        private GameTime spawnedTime;
        private Transform tgtTransform;

        [SerializeField]
        private GameObject AOEAttack;

        [SerializeField]
        private AttackData AOEAttackData;


        [SerializeField]
        private float trackTime;

        [SerializeField]
        private float triggerTime;

        private DynamicEntity Owner;

        private Vector3 initialScale;


        public void setOwner(DynamicEntity o)
        {
            this.Owner = o;
        }

        public void Start()
        {
            this.spawnedTime = GameTime.Now;
            this.initialScale = this.gameObject.transform.localScale;
            this.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            
            if (!this.spawnedTime.HasTimeElapsed(trackTime))
            {
                this.gameObject.transform.position = tgtTransform.position;

                this.gameObject.transform.localScale = Vector3.Lerp(
                    this.gameObject.transform.localScale,
                    this.initialScale,
                    0.04f
                );
            }
            if (this.spawnedTime.HasTimeElapsed(triggerTime))
            {
                Triggered();
            }
        }

        public void setTgtPos(Transform tgt)
        {
            this.tgtTransform = tgt;
        }

        public void Triggered()
        {
            Debug.Log("Triggered");
            Destroy(this.gameObject);
            GameObject attack = GameObject.Instantiate(AOEAttack, this.gameObject.transform.position, Quaternion.identity);
            attack.GetComponent<AOEAttackScript>().SetData(AOEAttackData);
            attack.GetComponent<AOEAttackScript>().StartAttack(Owner);
        }

    }
}
