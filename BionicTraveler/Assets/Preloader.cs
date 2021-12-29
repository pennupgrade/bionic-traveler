namespace BionicTraveler
{
    using System.Collections;
    using BionicTraveler.Scripts;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// Manages preloading of main game assets before the first scene gets displayed.
    /// Also displays an initial loading screen.
    /// This is only relevant for final builds of the game.
    /// </summary>
    public class Preloader : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameplayMainPrefab;

        [SerializeField]
        private GameObject splashScreen;

        [SerializeField]
        private string startScene;

        private bool hasSplashFinished;

        private void Start()
        {
            GameplayMainPersistence.SetInjectForNextScene(this.gameplayMainPrefab);
            this.StartCoroutine(this.LoadStartLevel());
            this.StartCoroutine(this.FadeInSplashscreen());
        }

        private IEnumerator LoadStartLevel()
        {
            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(this.startScene, LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;

            // Wait until the last operation fully loads to return anything.
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    if (this.hasSplashFinished)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            // Only activate DevConsole now.
            SickDev.DevConsole.DevConsole.singleton.Close();
        }

        private IEnumerator FadeInSplashscreen()
        {
            var image = this.splashScreen.GetComponent<Image>();
            image.canvasRenderer.SetAlpha(0.0f);
            yield return new WaitForSeconds(0.5f);
            image.CrossFadeAlpha(1.0f, 2.0f, true);
            yield return new WaitForSeconds(4.5f);
            image.CrossFadeAlpha(0.0f, 1.0f, true);
            yield return new WaitForSeconds(1.0f);
            this.hasSplashFinished = true;
        }
    }
}
