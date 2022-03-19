namespace BionicTraveler.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Main class to host game config.
    /// </summary>
    public class GameConfigManager : MonoBehaviour
    {
        [SerializeField]
        private GameConfig gameConfig;

        /// <summary>
        /// Gets the current game config.
        /// </summary>
        public GameConfig GameConfig => this.gameConfig;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Keep fields close to properties.")]
        public static GameConfigManager Instance { get; private set; }

        /// <summary>
        /// Gets the current config.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Keep fields close to properties.")]
        public static GameConfig CurrentConfig => Instance.GameConfig;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                Instance = this;
            }
        }
    }
}