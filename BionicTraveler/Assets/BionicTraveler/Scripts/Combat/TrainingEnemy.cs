namespace BionicTraveler.Scripts.Combat
{
    using System;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Interaction;
    using UnityEngine;

    /// <summary>
    /// Represents an enemy that can be used for training purposes by the player.
    /// </summary>
    public class TrainingEnemy : MonoBehaviour
    {
        [SerializeField]
        private GameObject spawnPrefab;

        /// <summary>
        /// Invoked when the player engaged this enemy.
        /// </summary>
        public event Action<TrainingEnemy> Engaged;

        /// <summary>
        /// Gets the prefab to spawn.
        /// </summary>
        public GameObject SpawnPrefab => this.spawnPrefab;

        private void Start()
        {
            // Disable our AI, we don't want them to engage.
            var entityBehavior = this.GetComponent<EntityBehavior>();
            if (entityBehavior != null)
            {
                entityBehavior.enabled = false;
            }

            this.GetComponent<DialogueHost>().DialogueCompleted += this.TrainingEnemy_DialogueCompleted;
        }

        private void TrainingEnemy_DialogueCompleted(DialogueHost sender)
        {
            if (sender.VariableStorage.GetValue("$decision_fight").AsBool)
            {
                this.Engaged?.Invoke(this);
            }
        }
    }
}
