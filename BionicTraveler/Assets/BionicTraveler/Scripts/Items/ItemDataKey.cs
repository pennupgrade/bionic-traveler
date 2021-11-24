namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;

    /// <summary>
    /// Describes an item that can be used as a key in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewKey", menuName = "Items/KeyData")]
    public class ItemDataKey : ItemData
    {
        [SerializeField]
        [Tooltip("The color of the key.")]
        private KeyColor color;

        /// <summary>
        /// Gets the color of the key.
        /// </summary>
        public KeyColor Color => this.color;
    }
}