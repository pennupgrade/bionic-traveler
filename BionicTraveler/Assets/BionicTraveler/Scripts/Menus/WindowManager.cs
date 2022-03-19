namespace BionicTraveler.Scripts.Menus
{
    // using BionicTraveler.Scripts;
    using UnityEngine;

    /// <summary>
    /// Manages the open and close of all menus.
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        private Menu openedMenu;

        /// <summary>
        /// Gets the singleton instance of WindowManager.
        /// </summary>
        public static WindowManager Instance { get; private set; }

        /// <summary>
        /// Update is called once per frame and checks if any menu open buttons have been called.
        /// If so, closes the current one if it exists and opens the new one.
        /// </summary>
        public void Update()
        {
            if (Input.GetButtonDown("Open Settings"))
            {
                this.ToggleMenu(SettingsManager.Instance);
            }
            else if (Input.GetButtonDown("Open Quests"))
            {
                this.ToggleMenu(QuestManager.Instance);
            }
            else if (Input.GetButtonDown("Open Map"))
            {
                this.ToggleMenu(MapManager.Instance);
            }
            else if (Input.GetButtonDown("Open Inventory"))
            {
                InventoryUI.Instance.SetUsePlayerInventory();
                this.ToggleMenu(InventoryUI.Instance);
            }
        }

        /// <summary>
        /// Called upon pressing a keyboard button related to a menu to toggle the visibility of the menu.
        /// </summary>
        /// <param name="menuInstance">The menu to toggle.</param>
        public void ToggleMenu(Menu menuInstance)
        {
            this.CloseCurrentMenu();

            if (this.openedMenu == menuInstance)
            {
                this.openedMenu = null;
            }
            else
            {
                this.openedMenu = menuInstance;
                this.openedMenu.Open();
            }
        }

        private void CloseCurrentMenu()
        {
            this.openedMenu?.Close();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
