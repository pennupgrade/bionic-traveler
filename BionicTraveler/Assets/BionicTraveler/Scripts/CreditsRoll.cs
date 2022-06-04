namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class CreditsRoll : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (gameObject.transform.position.y < 80)
            {
                var pos = gameObject.transform.position;
                gameObject.transform.position = new Vector3(pos.x, pos.y+.008f, pos.z);
            } 
            else
            {
                gameObject.transform.position = new Vector3(0, -80, 0);
            }
        }

        public void GoBack()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
