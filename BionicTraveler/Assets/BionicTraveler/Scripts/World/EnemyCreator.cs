namespace BionicTraveler.Scripts.World
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using WeightedDynEntity = Weighted<DynamicEntity>;

    /// <summary>
    /// Helper class to instantiate new <see cref="Pickup"/>s at runtime based on their prefab.
    /// </summary>
    public class EnemyCreator : MonoBehaviour
    {
        [SerializeField]
        private List<WeightedDynEntity> prefabSet;
        private int totalWeight = -1;

        /// <summary>
        /// Spawns a new enemy at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The newly created pickup.</returns>
        public static DynamicEntity SpawnEnemy(Vector3 position)
        {
            var enemyCreator = FindObjectOfType<EnemyCreator>();
            return enemyCreator.Spawn(position);
        }

        /// <summary>
        /// Spawns a new random enemy at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The newly created pickup.</returns>
        public DynamicEntity Spawn(Vector3 position)
        {
            var randomEnemy = this.PickRandomEnemy();
            var newEnemy = Instantiate(randomEnemy, position, new Quaternion(0, 0, 0, 0));

            // might want to set some properties on the new enemy here
            // newEnemy.SetDirection(Vector3.positiveInfinity);
            return newEnemy;
        }

        /// <summary>
        /// picks a random enemy from the set.
        /// the weights determine how likely some enemy is to get picked
        /// </summary>
        /// <returns>the DynamicEntity for the enemy</returns>
        /// <exception cref="InvalidOperationException">for what's supposed to be unreachable code</exception>
        private DynamicEntity PickRandomEnemy()
        {
            if (this.totalWeight <= 0)
            {
                this.totalWeight = 0;
                foreach (WeightedDynEntity wv in this.prefabSet)
                {
                    this.totalWeight += wv.Weight;
                }
            }

            System.Random rand = new System.Random();
            int sel = rand.Next() % this.totalWeight;
            int runningCount = 0;
            foreach (WeightedDynEntity wv in this.prefabSet)
            {
                runningCount += wv.Weight;
                if (sel < runningCount)
                {
                    return wv.Value;
                }
            }

            throw new InvalidOperationException("Shouldn't reach this!");
        }
    }
}