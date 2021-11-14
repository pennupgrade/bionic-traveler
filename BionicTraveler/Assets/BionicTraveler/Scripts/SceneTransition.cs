namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement; 

    /// <summary>
    /// Please document me.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {

        public string sceneToLoad;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                SceneManager.LoadScene(sceneToLoad); 
            }
        }
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>

    }
}
