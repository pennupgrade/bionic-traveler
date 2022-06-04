namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Manager class to handle spawning and processing enemy training.
    /// </summary>
    public class EnemyTrainingManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] enemies;

        [SerializeField]
        private GameObject playerSpawn;

        [SerializeField]
        private GameObject enemySpawn;

        private DialogueHost dialogueHost;

        [SerializeField]
        private YarnProgram dialogue;

        private string currentEnemyName;
        private bool hasEnemySpawned;
        private GameObject currentEnemy;

        private void Start()
        {
            foreach (var enemy in this.enemies)
            {
                var trainingEnemy = enemy.GetComponent<TrainingEnemy>();
                trainingEnemy.Engaged += this.TrainingEnemy_Engaged;
            }

            this.dialogueHost = this.GetComponent<DialogueHost>();
        }

        private void TrainingEnemy_Engaged(TrainingEnemy sender)
        {
            this.StartCoroutine(this.PrepareFight(sender));
        }

        private IEnumerator PrepareFight(TrainingEnemy enemy)
        {
            var fadeDuration = 1.5f;
            var newColor = new Color(1, 1, 1, 1);
            var startTime = Time.time;
            while (newColor.a > 0)
            {
                var fadeAmount = 1.0f - Mathf.Lerp(0.0f, 1.0f, (Time.time - startTime) / fadeDuration);
                newColor.a = fadeAmount;

                foreach (var e in this.enemies)
                {
                    if (e == enemy.gameObject)
                    {
                        continue;
                    }

                    var sprite = e.GetComponent<SpriteRenderer>();
                    sprite.color = newColor;
                }

                yield return null;
            }

            this.ToggleEnemyVisibility(false, enemy.gameObject);

            var player = GameObject.FindGameObjectWithTag("Player");
            var playerEntity = player.GetComponent<PlayerEntity>();
            playerEntity.IsIgnoredByEveryone = true;
            playerEntity.DisableInput();
            playerEntity.EnableScriptedSequenceMovement();

            // Make player go to start position.
            var playerTask = new TaskGoToPoint(playerEntity, this.playerSpawn.transform.position);
            playerTask.Assign();

            // Make enemy go to center of arena.
            var taskGoTo = new TaskGoToPoint(enemy.GetComponent<DynamicEntity>(), this.enemySpawn.transform.position);
            taskGoTo.Assign();

            var timeout = 8.0f;
            var walkStart = Time.time;
            while (!taskGoTo.HasEnded)
            {
                if (Time.time - walkStart > timeout)
                {
                    break;
                }

                yield return null;
            }

            // Spawn new enemy.
            enemy.gameObject.SetActive(false);
            var newEnemy = Instantiate(enemy.SpawnPrefab, enemy.transform.position, enemy.transform.rotation);

            // Ready player.
            playerEntity.DisableScriptedSequenceMovement();
            playerEntity.EnableInput();
            playerEntity.IsIgnoredByEveryone = false;
            playerEntity.Damaged += this.PlayerEntity_Damaged;
            newEnemy.GetComponent<DynamicEntity>().Damaged += this.EnemyTrainingManager_Damaged;
            this.currentEnemy = newEnemy;
            this.hasEnemySpawned = true;

            // Dialogue.
            this.currentEnemyName = enemy.GetComponent<DialogueHost>().LastName;
            this.dialogueHost.StartDialogue(player, this.dialogue.name, this.currentEnemyName, "enemy_start");
        }

        private void PlayerEntity_Damaged(Entity sender, Entity attacker, bool fatal)
        {
            // Intercept death for a dialogue.
            if (fatal)
            {
                sender.SetHealth(100);
                attacker.GetComponent<EntityBehavior>().enabled = false;
                attacker.GetComponent<DynamicEntity>().TaskManager.ClearTasks();
                var player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerEntity>().Damaged -= this.PlayerEntity_Damaged;

                this.dialogueHost.DialogueCompleted += host =>
                {
                    LevelLoadingManager.Instance.ReloadCurrentScene();
                };

                this.dialogueHost.StartDialogue(player, this.dialogue.name, this.currentEnemyName, "enemy_won");
            }
        }

        private void EnemyTrainingManager_Damaged(Entity sender, Entity attacker, bool fatal)
        {
            // Intercept death for a dialogue.
            if (fatal)
            {
                sender.SetHealth(100);
                sender.GetComponent<EntityBehavior>().enabled = false;
                sender.GetComponent<DynamicEntity>().TaskManager.ClearTasks();
                var player = GameObject.FindGameObjectWithTag("Player");

                this.dialogueHost.DialogueCompleted += host =>
                {
                    sender.Died += (entity, killer) =>
                    {
                        LevelLoadingManager.Instance.ReloadCurrentScene();
                    };

                    this.hasEnemySpawned = false;
                    sender.Kill();
                };

                this.dialogueHost.StartDialogue(player, this.dialogue.name, this.currentEnemyName, "enemy_lost");
            }
        }

        private void ToggleEnemyVisibility(bool visible, GameObject except)
        {
            foreach (var enemy in this.enemies)
            {
                if (enemy == except)
                {
                    continue;
                }

                enemy.SetActive(visible);
            }
        }

        private void Update()
        {
            // Some enemies, such as the bomber, kill themselves with an animation.
            // Hence we check for their existence to determine whether the fight is over.
            if (this.hasEnemySpawned && this.currentEnemy.IsDestroyed())
            {
                this.hasEnemySpawned = false;

                var player = GameObject.FindGameObjectWithTag("Player");
                this.dialogueHost.DialogueCompleted += host =>
                {
                    LevelLoadingManager.Instance.ReloadCurrentScene();
                };

                this.dialogueHost.StartDialogue(player, this.dialogue.name, this.currentEnemyName, "enemy_lost");
            }
        }
    }
}
