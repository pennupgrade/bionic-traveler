using BionicTraveler.Scripts.World;

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
        private bool isOnTreasure;
        private DynamicEntity dynamicEntity;

        /// <summary>
        /// Sets instance of SaveManager and adds the Treasure.cs's save and load functions
        /// to SaveManager's list of functions to call when saving and loading.
        /// </summary>
        private void Start()
        {
            this.saveManager = SaveManager.Instance;
            this.saveManager.IsSaving += this.Save;
            this.saveManager.IsLoading += this.Load;
            this.rendererComponent = this.GetComponent<Renderer>();
            this.dynamicEntity = this.GetComponent<DynamicEntity>();
            this.Load();
        }

        private void OnDestroy()
        {
            // Always free your events unless you want the GC to keep some partially disposed objects
            // around (hint: you never really want that).
            this.saveManager.IsSaving -= this.Save;
            this.saveManager.IsLoading -= this.Load;
        }

        /// <summary>
        /// When player collides with treasure, change bool status.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                this.isOnTreasure = true;
                // this.rendererComponent.enabled = false;
            }
        }

        /// <summary>
        /// When player exits treasure, change bool status.
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                this.isOnTreasure = false;
            }
        }

        /// <summary>
        /// If player is on treasure and then presses L, treasure will be taken.
        /// </summary>
        private void Update()
        {
            if (this.isOnTreasure && Input.GetKeyDown(KeyCode.L))
            {
                WindowManager.Instance.ToggleMenu(InventoryUI.Instance, this.dynamicEntity);
                // WindowManager.Instance.ToggleMenu(InventoryUI.Instance);
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
