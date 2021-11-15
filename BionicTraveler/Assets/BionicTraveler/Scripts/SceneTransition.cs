namespace BionicTraveler.Scripts
{
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Transitions into a new scene on trigger.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField]
        private string sceneToLoad;

        [SerializeField]
        private string spawnPointOverride;

        private bool justSpawnedPlayer;
        private GameObject player;

        /// <summary>
        /// Called when the player just got spawned at this transition location.
        /// </summary>
        /// <param name="player">The player.</param>
        public void OnSpawnedPlayer(GameObject player)
        {
            this.justSpawnedPlayer = true;
            this.player = player;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger && !LevelLoadingManager.Instance.IsLoading &&
                !this.justSpawnedPlayer)
            {
                var spawnPoint = !string.IsNullOrWhiteSpace(this.spawnPointOverride) ? this.spawnPointOverride
                    : LevelLoadingManager.DefaultSpawnPoint;
                LevelLoadingManager.Instance.StartLoadLevel(this.sceneToLoad, spawnPoint);
            }
        }

        private void FixedUpdate()
        {
            // If player just spawned here, they need to get away a little bit before
            // they can enter the trigger again.
            if (this.justSpawnedPlayer)
            {
                if (this.transform.DistanceTo(this.player.transform) > 2)
                {
                    this.justSpawnedPlayer = false;
                }
            }
        }
    }
}