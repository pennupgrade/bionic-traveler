namespace BionicTraveler.Scripts
{
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger && !LevelLoadingManager.Instance.IsLoading)
            {
                var spawnPoint = !string.IsNullOrWhiteSpace(this.spawnPointOverride) ? this.spawnPointOverride
                    : LevelLoadingManager.DefaultSpawnPoint;
                LevelLoadingManager.Instance.StartLoadLevel(this.sceneToLoad, spawnPoint);
            }
        }
    }
}