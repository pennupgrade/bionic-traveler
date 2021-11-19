namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Manages spawning of the player in a scene.
    /// </summary>
    public class PlayerSpawnController : MonoBehaviour
    {
        [SerializeField]
        private GameObject player;

        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField]
        private List<GameObject> spawnsLocations;

        private void Awake()
        {
            var spawnPoint = this.GetSpawnPoint();
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                this.player = GameObject.FindGameObjectWithTag("Player");

                // If we have an existing player and are loading a scene, adjust player position.
                // Otherwise, we got started from the editor and manual player position should take precedence.
                if (LevelLoadingManager.Instance.IsLoading)
                {
                    this.player.transform.position = spawnPoint.transform.position;
                    Debug.Log("PlayerSpawnController::Awake: Initialized existing player to spawn point");
                }
                else
                {
                    Debug.Log("PlayerSpawnController::Awake: Initialized existing player to prefab location");
                }
            }
            else
            {
                this.player = Instantiate(this.playerPrefab);
                this.player.name = "Player_Spawned";
                this.player.transform.position = spawnPoint.transform.position;
                Debug.Log("PlayerSpawnController::Awake: Initialized new player");
            }

            // If our spawn point is also a transition, tell it we just spawned there to prevent the
            // player from triggering it again immediately.
            spawnPoint.GetComponent<SceneTransition>()?.OnSpawnedPlayer(this.player);
        }

        private GameObject GetSpawnPoint()
        {
            // For default, we spawn where the controller is located.
            var spawnPoint = LevelLoadingManager.Instance.DesiredSpawnPoint;
            if (spawnPoint == LevelLoadingManager.DefaultSpawnPoint)
            {
                return this.gameObject;
            }

            var location = this.spawnsLocations.FirstOrDefault(loc => loc.name == spawnPoint);
            if (location == null)
            {
                throw new InvalidOperationException($"Cannot find spawn point {spawnPoint}. Did you" +
                    $" add the spawn point to the spawn controller locations list?");
            }

            return location;
        }
    }
}
