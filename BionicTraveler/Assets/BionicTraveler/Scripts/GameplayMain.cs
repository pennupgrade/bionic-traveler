namespace BionicTraveler.Scripts
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Placeholder script that lives on the GameplayMain object to act as a reference.
    /// This script is checked for existence by the scene loader to establish whether a new GameplayMain
    /// prefab instance should be instantiated. Do not remove this script.
    /// See <see cref="GameplayMainPersistence"/>.
    /// </summary>
    public class GameplayMain : MonoBehaviour
    {
        /// <summary>
        /// Gets a value indicating whether the current session was just launched from the editor.
        /// </summary>
        public static bool WasJustLaunchedFromEditor { get; private set; }

        private void Awake()
        {
            Debug.Log("GameplayMain::Awake: Hello World");
            GameplayMain.WasJustLaunchedFromEditor = EditorPrefs.GetBool("JustLaunchedFromEditor");
        }

        private void Start()
        {
            GameplayMain.WasJustLaunchedFromEditor = false;
        }
    }
}