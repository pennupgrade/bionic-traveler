using BionicTraveler.Scripts;
using UnityEngine;

namespace BionicTraveler
{
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
                if (this.openedMenu)
                {
                    this.openedMenu.Close();
                }

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
                if (this.openedMenu)
                {
                    this.openedMenu.Close();
                }

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
        }
    }
}
