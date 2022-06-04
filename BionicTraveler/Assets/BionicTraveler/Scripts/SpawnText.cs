namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class SpawnText : MonoBehaviour
    {
        private Animator logAnimator;
        private bool spawned;
        public GameObject canvas;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            logAnimator = gameObject.GetComponent<Animator>();
            spawned = false;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            AnimatorStateInfo currState = logAnimator.GetCurrentAnimatorStateInfo(0);
            if (!spawned && currState.IsName("Idle"))
            {
                Instantiate(canvas);
                spawned = true;
            }
        }
    }
}
