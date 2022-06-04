namespace BionicTraveler.Scripts
{
    using System.Collections;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// Transitions into a new scene on trigger.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The name of the scene to load.")]
        private string sceneToLoad;

        [SerializeField]
        [Tooltip("The name of the game object that this transition should lead to.")]
        private string spawnPointOverride;

        [SerializeField]
        [Tooltip("The direction to slide the player in.")]
        private SlideDirection slideDirection;

        [SerializeField]
        private bool slidePlayer;

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

            if (this.slidePlayer)
            {
                var dir = this.GetSlideVector(this.slideDirection);
                this.player.GetComponent<PlayerEntity>().SetDirection(this.player.transform.position + dir);
                this.StartCoroutine(this.Transition(dir, 1.5f));
            }
        }

        private Vector3 GetSlideVector(SlideDirection slideDirection)
        {
            switch (slideDirection)
            {
                case SlideDirection.Left:
                    return Vector3.left;
                case SlideDirection.Right:
                    return Vector3.right;
                case SlideDirection.Up:
                    return Vector3.up;
                case SlideDirection.Down:
                    return Vector3.down;
                default:
                    return Vector3.zero;
            }
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

        // Slowly slides the player in an input direction for an input distance
        private IEnumerator Transition(Vector3 dir, float dist)
        {
            Transform t = this.player.transform;
            float i = 0f;
            this.player.GetComponent<PlayerEntity>().DisableInput();

            while (i < dist)
            {
                i += Time.deltaTime;
                t.position += 2 * Time.deltaTime * dir;
                yield return null;
            }

            this.player.GetComponent<PlayerEntity>().EnableInput();
        }
    }
}