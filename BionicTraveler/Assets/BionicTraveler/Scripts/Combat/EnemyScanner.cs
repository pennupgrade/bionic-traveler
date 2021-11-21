namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Finds all of the entity's enemies.
    /// </summary>
    public class EnemyScanner
    {
        private readonly EntityRelationships relationships;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyScanner"/> class.
        /// </summary>
        /// <param name="relationships">Entity's relationships.</param>
        public EnemyScanner(EntityRelationships relationships)
        {
            this.relationships = relationships ?? throw new ArgumentNullException(nameof(relationships));
        }

        /// <summary>
        /// Gets an array of enemies based on their relation to owner.
        /// </summary>
        /// <returns>An array of enemies.</returns>
        public GameObject[] GetEnemies()
        {
            List<GameObject> enemies = new List<GameObject>();
            foreach (string enemy in this.relationships.Enemies)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags.Contains(enemy))
                {
                    enemies.AddRange(GameObject.FindGameObjectsWithTag(enemy).Where(
                    obj => obj.GetComponent<DynamicEntity>() != null));
                }
                else
                {
                    Debug.LogWarning($"Tag {enemy} does not exist.");
                }
            }

            return enemies.ToArray();
        }
    }
}
