namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using BionicTraveler.Scripts.Audio;

    /// <summary>
    /// Interface that all menus must implement. Contains an open and close method.
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        private Canvas canvas; // Canvas containing the menu

        [SerializeField]
        private AudioClip openSound;
        [SerializeField]
        private AudioClip closeSound;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public virtual void Start()
        {
            this.canvas = this.GetComponent<Canvas>();
            this.canvas.enabled = false;
            Time.timeScale = 1;
        }

        /// <summary>
        /// Opens the menu canvas and pauses the game
        /// </summary>
        public void Open()
        {
            
            AudioManager.Instance.PlayOneShot(openSound);
            this.canvas.enabled = true;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Closes the menu canvas and resumes the game
        /// </summary>
        public void Close()
        {
            
            AudioManager.Instance.PlayOneShot(closeSound);
            this.canvas.enabled = false;
            Time.timeScale = 1;
        }
    }
}
