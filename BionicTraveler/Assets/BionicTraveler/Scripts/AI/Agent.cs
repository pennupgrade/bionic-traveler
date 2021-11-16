namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class Agent : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        private NavMeshAgent agent;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.target = GameObject.FindGameObjectWithTag("Player").transform;

            this.agent = this.GetComponent<NavMeshAgent>();
            this.agent.updateRotation = false;
            this.agent.updateUpAxis = false;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            this.agent.SetDestination(this.target.position);
        }
    }
}
