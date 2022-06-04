namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages loading new game levels and ensures that state is transferred correctly.
    /// </summary>
    public class LevelLoadingManager : MonoBehaviour
    {
        /// <summary>
        /// The name of the default spawn point.
        /// </summary>
        public const string DefaultSpawnPoint = "default_sp";

        private static LevelLoadingManager instance;

        [SerializeField]
        [Tooltip("The loading screen game object to use.")]
        private GameObject loadingScreen;

        private string sceneToLoad;

        /// <summary>
        /// Called whenever a new level has been loaded in.
        /// </summary>
        public event Action FinishedLoading;

        /// <summary>
        /// Gets the level loading manager instance.
        /// </summary>
        public static LevelLoadingManager Instance => LevelLoadingManager.instance;

        /// <summary>
        /// Gets the next desired spawn point for the player.
        /// </summary>
        public string DesiredSpawnPoint { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a new level is currently being loaded.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Gets the name of the current scene.
        /// </summary>
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        private void Awake()
        {
            if (LevelLoadingManager.instance != null && LevelLoadingManager.instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                LevelLoadingManager.instance = this;
            }

            this.sceneToLoad = SceneManager.GetActiveScene().name;
            this.DesiredSpawnPoint = LevelLoadingManager.DefaultSpawnPoint;
        }

        /// <summary>
        /// Starts loading <paramref name="scene"/>.
        /// </summary>
        /// <param name="scene">The name of the scene.</param>
        public void StartLoadLevel(string scene)
        {
            this.StartLoadLevel(scene, LevelLoadingManager.DefaultSpawnPoint);
        }

        /// <summary>
        /// Starts loading <paramref name="scene"/> and places the player at <paramref name="spawnPoint"/>.
        /// </summary>
        /// <param name="scene">The name of the scene.</param>
        /// <param name="spawnPoint">The name of the spawnpoint.</param>
        public void StartLoadLevel(string scene, string spawnPoint)
        {
            this.sceneToLoad = scene;
            this.DesiredSpawnPoint = spawnPoint;
            this.IsLoading = true;
            Time.timeScale = 0;
            this.loadingScreen.SetActive(true);
            this.StartCoroutine(this.LoadSceneAsync());
        }

        /// <summary>
        /// Reloads the currently active scene.
        /// </summary>
        public void ReloadCurrentScene()
        {
            this.StartCoroutine(this.ReloadCurrentSceneAsync());
        }

        private IEnumerator ReloadCurrentSceneAsync()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            var currentSceneName = currentScene.name;
            var currentPlayer = GameObject.FindGameObjectWithTag("Player");

            this.IsLoading = true;
            Time.timeScale = 0;
            this.loadingScreen.SetActive(true);

            // Activate loading screen scene.
            var loadingScreenSceneName = "LoadingScreen";
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadingScreenSceneName, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything.
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            var loadingScreenScene = SceneManager.GetSceneByName(loadingScreenSceneName);
            SceneManager.MoveGameObjectToScene(currentPlayer, loadingScreenScene);
            currentPlayer = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Player: " + currentPlayer == null);

            // Unload and reload current scene.
            asyncLoad = SceneManager.UnloadSceneAsync(currentScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            asyncLoad = SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything.
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            var newScene = SceneManager.GetSceneByName(currentSceneName);
            SceneManager.MoveGameObjectToScene(currentPlayer, newScene);

            // Unload the previous Scene.
            asyncLoad = SceneManager.UnloadSceneAsync(loadingScreenScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Time.timeScale = 1;
            this.loadingScreen.SetActive(false);
            this.IsLoading = false;
            this.FinishedLoading?.Invoke();
            Debug.Log("Old level unloaded");

            // In case something disabled player input, we re-enable it.
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerEntity>().EnableInput();
        }

        private IEnumerator LoadSceneAsync()
        {
            // Get the current Scene to be able to unload it later.
            Scene currentScene = SceneManager.GetActiveScene();
            var currentPlayer = GameObject.FindGameObjectWithTag("Player");
            Debug.Log($"Starting loading {this.sceneToLoad} from {currentScene.name}");

            // Allow the loading screen to become active. If we do not yield here, the loading below
            // might make Unity busy and our screen shows up late.
            yield return null;

            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(this.sceneToLoad, LoadSceneMode.Additive);

            // Wait until the last operation fully loads to return anything.
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Move our player to the new scene.
            var newScene = SceneManager.GetSceneByName(this.sceneToLoad);
            SceneManager.MoveGameObjectToScene(currentPlayer, newScene);
            Debug.Log("Level loaded and initialized!");

            // Unload the previous Scene.
            asyncLoad = SceneManager.UnloadSceneAsync(currentScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Time.timeScale = 1;
            this.loadingScreen.SetActive(false);
            this.IsLoading = false;
            this.FinishedLoading?.Invoke();
            Debug.Log("Old level unloaded");
        }
    }
}
