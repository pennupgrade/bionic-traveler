namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Interface that all menus must implement. Contains an open and close method.
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        private Canvas canvas; // Canvas containing the menu

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public virtual void Start()
        {
            this.canvas = this.GetComponent<Canvas>();
            if (this.canvas != null)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Opens the menu canvas and pauses the game
        /// </summary>
        public virtual void Open()
        {
            this.canvas.enabled = true;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Closes the menu canvas and resumes the game
        /// </summary>
        public virtual void Close()
        {
            this.canvas.enabled = false;
            Time.timeScale = 1;
        }
    }
}
