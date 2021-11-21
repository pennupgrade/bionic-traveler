namespace BionicTraveler.Scripts.AI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class describes relationships between entities; indicates which entities are attacked on sight.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewRelationship", menuName = "AI/Relationship Data")]
    public class EntityRelationships : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Tags of friendly entities.")]
        private List<string> friends;

        [SerializeField]
        [Tooltip("Tags of hostile entities.")]
        private List<string> enemies;

        /// <summary>
        /// Gets the hostile tags.
        /// </summary>
        public List<string> Enemies => this.enemies;

        /// <summary>
        /// Checks whether the tag is an enemy.
        /// </summary>
        /// <param name="tag">The tag that is checked.</param>
        /// <returns>True if tag is an enemy; false otherwise.</returns>
        public bool IsHostile(string tag)
        {
            return this.enemies.Contains(tag);
        }
    }
}
