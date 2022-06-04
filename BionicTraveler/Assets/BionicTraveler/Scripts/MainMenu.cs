namespace BionicTraveler.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// The main menu logic.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup menu;

        [SerializeField]
        private TMPro.TMP_Dropdown sceneDropdown;

        private string[] sceneOptions;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.sceneDropdown.ClearOptions();

            var optionDataList = new List<TMPro.TMP_Dropdown.OptionData>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                if (name.Contains("Preload") || name.Contains("MainMenu") || name.Contains("LoadingScreen"))
                {
                    continue;
                }

                optionDataList.Add(new TMPro.TMP_Dropdown.OptionData(name));
            }

            this.sceneOptions = optionDataList.Select(x => x.text).ToArray();
            this.sceneDropdown.AddOptions(optionDataList);
        }

        public void LoadLevel()
        {
            this.LoadLevel(this.sceneOptions[this.sceneDropdown.value]);
        }

        public void StartGame()
        {
            this.LoadLevel("LandscapeScene");
        }

        public void StartBattleArena()
        {
            this.LoadLevel("EnemyTrainingPlatform");
        }

        public void LoadArtGallery()
        {
            this.LoadLevel("ArtGallery");
        }

        public void LoadFanFic()
        {
            this.LoadLevel("FanFic");
        }

        public void LoadCredits()
        {
            this.LoadLevel("Credits Roll");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game.
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        private void LoadLevel(string level)
        {
            this.menu.interactable = false;
            this.StartCoroutine(this.LoadNewLevel(level));
        }

        private IEnumerator LoadNewLevel(string level)
        {
            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;

            // Wait until the last operation fully loads to return anything.
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
