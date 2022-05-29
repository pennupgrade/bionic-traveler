namespace BionicTraveler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Interaction;
    using UnityEngine;

    /// <summary>
    /// Represents an enemy that can be used for training purposes by the player.
    /// </summary>
    public class TrainingEnemy : MonoBehaviour
    {
        private void Start()
        {
            // Disable our AI, we don't want them to engage.
            this.GetComponent<EntityBehavior>().enabled = false;
            this.GetComponent<DialogueHost>().DialogueCompleted += this.TrainingEnemy_DialogueCompleted;
        }

        private void TrainingEnemy_DialogueCompleted(DialogueHost sender)
        {
            if (sender.VariableStorage.GetValue("$decision_fight").AsBool)
            {
                
            }
        }
    }
}
