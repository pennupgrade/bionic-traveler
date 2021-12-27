namespace BionicTraveler
{
    using BionicTraveler.Scripts;
    using UnityEngine;

    /// <summary>
    /// Manages the open and close of all menus.
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        private Menu openedMenu;

        /// <summary>
        /// Update is called once per frame and checks if any menu open buttons have been called.
        /// If so, closes the current one if it exists and opens the new one.
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.CloseCurrentMenu();

                if (this.openedMenu == SettingsManager.Instance)
                {
                    this.openedMenu = null;
                }
                else
                {
                    this.openedMenu = SettingsManager.Instance;
                    this.openedMenu.Open();
                }
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                this.CloseCurrentMenu();

                if (this.openedMenu == QuestManager.Instance)
                {
                    this.openedMenu = null;
                }
                else
                {
                    this.openedMenu = QuestManager.Instance;
                    this.openedMenu.Open();
                }
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                this.CloseCurrentMenu();

                if (this.openedMenu == MapManager.Instance)
                {
                    this.openedMenu = null;
                }
                else
                {
                    this.openedMenu = MapManager.Instance;
                    this.openedMenu.Open();
                }
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                this.CloseCurrentMenu();

                if (this.openedMenu == InventoryUI.Instance)
                {
                    this.openedMenu = null;
                }
                else
                {
                    this.openedMenu = InventoryUI.Instance;
                    this.openedMenu.Open();
                }
            }
        }

        private void CloseCurrentMenu()
        {
            this.openedMenu?.Close();
        }
    }
}
