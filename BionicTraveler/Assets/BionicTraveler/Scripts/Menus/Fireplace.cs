namespace BionicTraveler.Scripts.Menus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Script attached to fireplace objects to open up save.
    /// </summary>
    public class Fireplace : MonoBehaviour
    {
        /// <summary>
        /// If player is on fireplace.
        /// </summary>
        private bool isOnFireplace;

        /// <summary>
        /// When player collides with fireplace, change bool status.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                this.isOnFireplace = true;
            }
        }

        /// <summary>
        /// When player collides with fireplace, change bool status.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                this.isOnFireplace = false;
            }
        }

        /// <summary>
        /// If player is on fireplace and then presses L, the save menu will open.
        /// </summary>
        private void Update()
        {
            if (this.isOnFireplace && Input.GetKeyDown(KeyCode.L))
            {
                WindowManager.Instance.ToggleMenu(SaveManager.Instance);
            }
        }
    }
}
