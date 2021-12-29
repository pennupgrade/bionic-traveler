namespace BionicTraveler.Scripts
{
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Hooks into scene loading and instantiates the GameplayMain prefab for a scene if it is missing.
    /// This only applies to scenes loaded directly from the editor. For final builds, <see cref="PlayerSpawnController"/>
    /// calls <see cref="EnsureLoaded"/> since unfortunately BeforeScene loaded is only fired once per application run.
    /// </summary>
    internal class GameplayMainPersistence
    {
        /// <summary>
        /// Ensures the prefab is loaded.
        /// </summary>
        public static void EnsureLoaded()
        {
            OnBeforeSceneLoad();
        }

        /// <summary>
        /// NOTE: BeforeSceneLoad is only called once per application run, so for final builds
        /// this is only called for the Preload scene and hence this function never works as intended
        /// and relies on <see cref="EnsureLoaded"/> to inject our prefab accordingly.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // Do not inject when we are on the Preload scene as we do not want any player related
            // stuff loaded just then.
            var isPreloadScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Preload";
            if (isPreloadScene)
            {
                return;
            }

#if UNITY_EDITOR
            // Only inject when we are being launched from the editor.
            var editorLaunch = UnityEditor.EditorPrefs.GetBool("JustLaunchedFromEditor");
            if (editorLaunch)
            {
                Debug.Log("GameplayMainPersistence::OnBeforeSceneLoad: Editor launch detected");
            }
#endif

            // If we cannot find our prefab, inject it. Otherwise, do nothing.
            var gameObject = UnityEngine.Object.FindObjectsOfType<GameplayMain>().FirstOrDefault();
            if (gameObject == null)
            {
                // Throw if prefab is not valid.
                var asset = Resources.Load<GameObject>("GameplayMain");
                if (asset == null)
                {
                    throw new InvalidOperationException("No GameplayMain prefab defined to inject!");
                }

                var obj = GameObject.Instantiate(asset);
                obj.name = "GameplayMain_Injected";
                Debug.Log("Added new GameplayMain prefab instance to scene");
            }
            else
            {
                Debug.Log("GameplayMain already exists on scene");
            }
        }
    }
}
