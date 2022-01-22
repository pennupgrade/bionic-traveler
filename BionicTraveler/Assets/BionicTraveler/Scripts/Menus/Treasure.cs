namespace BionicTraveler.Scripts.Menus
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// EXAMPLE CLASS ON HOW TO SAVE TO STORE
    /// </summary>
    public class Treasure : MonoBehaviour
    {
        private Renderer rendererComponent;
        private SaveManager saveManager;

        /// <summary>
        /// Sets instance of SaveManager and adds the Treasure.cs's save and load functions
        /// to SaveManager's list of functions to call when saving and loading.
        /// </summary>
        private void Start()
        {
            this.saveManager = SaveManager.Instance;
            Debug.Log(SaveManager.Instance);
            this.saveManager.IsSaving += this.Save;
            this.saveManager.IsLoading += this.Load;
            this.rendererComponent = this.GetComponent<Renderer>();
            this.Load();
        }

        /// <summary>
        /// Treasure disappears if player collides with it.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                this.rendererComponent.enabled = false;
            }
        }

        /// <summary>
        /// When saving, function will be called to save the state of whether the treasure has been taken.
        /// </summary>
        private void Save()
        {
            this.saveManager.Save("TreasureTaken", !this.rendererComponent.enabled);
        }

        /// <summary>
        /// When loading, function will be called to load the state of whether the treasure has been taken.
        /// </summary>
        private void Load()
        {
            var treasureTaken = this.saveManager.Load("TreasureTaken");
            this.rendererComponent.enabled = treasureTaken == null || !((bool)treasureTaken);
        }
    }
}
