namespace BionicTraveler.Scripts.World
{
    using System;
    using System.Collections.Generic;
    using BionicTraveler.Assets.Framework;
    using UnityEngine;
    using WeightedDynEntity = Weighted<DynamicEntity>;

    /// <summary>
    /// Helper class to instantiate new <see cref="Pickup"/>s at runtime based on their prefab.
    /// </summary>
    public class EnemyCreator : MonoBehaviour
    {
        private readonly System.Random random = new System.Random();

        [SerializeField]
        private List<WeightedDynEntity> prefabSet;

        [SerializeField]
        private int maxEnemies;

        [SerializeField]
        private int spawnRadius;

        [SerializeField]
        private bool autoSpawn;

        [SerializeField]
        private bool allowSpawnWithinCamera;

        [SerializeField]
        private float spawnProbeIntervalSeconds;

        // number of enemies already spawned/currently on scene
        private int numEnemies;
        private int totalWeight = -1;
        private GameTime lastSpawnCheck;

        private EnemyCreator()
        {
            this.autoSpawn = true;
            this.allowSpawnWithinCamera = false;
            this.spawnProbeIntervalSeconds = 2.5f;
        }

        /// <summary>
        /// Spawns all enemis up to the maximum.
        /// </summary>
        public void SpawnAllNear()
        {
            while (this.numEnemies < this.maxEnemies)
            {
                this.SpawnNear();
            }
        }

        /// <summary>
        /// Spawn's an enemy near the EnemyCreator, within the radius.
        /// </summary>
        /// <returns>the created dynamic entity</returns>
        public DynamicEntity SpawnNear()
        {
            var newPosition = this.transform.position;
            newPosition += (Vector3)(UnityEngine.Random.insideUnitCircle * this.spawnRadius);
            return this.SpawnAt(newPosition);
        }

        /// <summary>
        /// Spawns a new random enemy at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The newly created pickup.</returns>
        public DynamicEntity SpawnAt(Vector3 position)
        {
            if (this.numEnemies >= this.maxEnemies)
            {
                return null;
            }

            var randomEnemy = this.PickRandomEnemy();
            var newEnemy = Instantiate(randomEnemy, position, new Quaternion(0, 0, 0, 0));
            this.numEnemies += 1;

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

            int sel = this.random.Next() % this.totalWeight;
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

        private void Start()
        {
            this.lastSpawnCheck = GameTime.Default;
        }

        private void Update()
        {
            if (this.lastSpawnCheck.HasTimeElapsed(this.spawnProbeIntervalSeconds))
            {
                if (this.allowSpawnWithinCamera || !this.IsOnScreen())
                {
                    this.SpawnNear();
                    this.lastSpawnCheck = GameTime.Now;
                }
            }
        }

        private bool IsOnScreen()
        {
            var screenPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);
            return screenPos.x > 0 && screenPos.x < 1.0 && screenPos.y > 0 && screenPos.y < 1.0;
        }

        /// <summary>
        /// supposed to draw a sphere showing the spawn area.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, this.spawnRadius);
        }
    }
}