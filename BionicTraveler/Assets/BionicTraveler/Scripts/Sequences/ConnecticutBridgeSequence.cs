namespace BionicTraveler.Scripts.Sequences
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// The sequence responsible for spawning the bridge and making a new NPC appear and walk to the player.
    /// </summary>
    public class ConnecticutBridgeSequence : MonoBehaviour
    {
        private enum BridgeState
        {
            Waiting,
            SpawningBridge,
            PlayerWalking,
            NPCWalking,
            Finished,
        }

        [SerializeField]
        private GameObject playerMoveToPos;

        [SerializeField]
        private GameObject bridgePrefab;

        [SerializeField]
        private GameObject npcSpawnPos;

        [SerializeField]
        private GameObject npcSpawnPrefab;

        [SerializeField]
        private GameObject npcMovePos;

        [SerializeField]
        private YarnProgram npcMoveDialogue;

        private TaskGoToPoint playerMoveTask;
        private TaskGoToPoint npcMoveTask;
        private GameObject npcSpawned;
        private BridgeState state;

        private void Start()
        {
            this.state = BridgeState.Waiting;
        }

        private void Update()
        {
            switch (this.state)
            {
                case BridgeState.PlayerWalking:
                    if (this.playerMoveTask != null && this.playerMoveTask.HasEnded)
                    {
                        // Spawn NPC.
                        this.npcSpawned = Instantiate(this.npcSpawnPrefab, this.npcSpawnPos.transform.position, Quaternion.identity);
                        this.npcMoveTask = new TaskGoToPoint(this.npcSpawned.GetComponent<DynamicEntity>(), this.npcMovePos.transform.position);
                        this.npcMoveTask.Assign();
                        this.state = BridgeState.NPCWalking;
                    }

                    break;

                case BridgeState.NPCWalking:
                    if (this.npcMoveTask != null && this.npcMoveTask.HasEnded)
                    {
                        // Start dialogue. The dialogue will give back control to the player
                        // automatically when finished.
                        var dialogueHost = this.GetComponent<DialogueHost>();
                        var player = GameObject.FindGameObjectWithTag("Player");
                        var playerEntity = player.GetComponent<PlayerEntity>();
                        playerEntity.DisableScriptedSequenceMovement();
                        dialogueHost.StartDialogue(player, this.npcMoveDialogue.name);
                        this.state = BridgeState.Finished;
                    }

                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                var player = collision.gameObject.GetComponent<PlayerEntity>();
                player.DisableInput();
                player.EnableScriptedSequenceMovement();

                this.playerMoveTask = new TaskGoToPoint(player, this.playerMoveToPos.transform.position);
                this.playerMoveTask.Assign();

                this.state = BridgeState.PlayerWalking;
            }
        }
    }
}
