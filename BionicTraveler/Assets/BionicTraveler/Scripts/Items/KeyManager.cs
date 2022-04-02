namespace BionicTraveler.Scripts.Items
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Describes the key color.
    /// </summary>
    public enum KeyColor
    {
        /// <summary>
        /// Red key.
        /// </summary>
        Red,

        /// <summary>
        /// Yellow key.
        /// </summary>
        Yellow,

        /// <summary>
        /// Blue key.
        /// </summary>
        Blue,
    }

    /// <summary>
    /// Manages special keys the player has collected.
    /// </summary>
    public class KeyManager
    {
        private List<KeyData> keyData;

        public List<KeyData> KeyData { get { return keyData; } }


        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManager"/> class.
        /// </summary>
        public KeyManager()
        {
            this.keyData = new List<KeyData>();
        }

        public KeyManager(List<KeyData> data)
        {
            this.keyData = data;
        }

        /// <summary>
        /// Adds a key based on an item.
        /// </summary>
        /// <param name="item">The key item.</param>
        public void AddKey(ItemDataKey item)
        {
            var currentScene = LevelLoadingManager.Instance.CurrentSceneName;
            this.keyData.Add(new KeyData(item.Color, new string[] { currentScene }));
        }

        /// <summary>
        /// Gets all keys for <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Array of keys.</returns>
        public KeyData[] GetKeysForLevel(string name)
        {
            return this.keyData.Where(key => key.Scenes.Contains(name)).ToArray();
        }

        /// <summary>
        /// Gets all keys for the current level.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Array of keys.</returns>
        public KeyData[] GetKeysForCurrentLevel()
        {
            return this.GetKeysForLevel(LevelLoadingManager.Instance.CurrentSceneName);
        }
    }
}