namespace BionicTraveler.Scripts.Interaction
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A dialogue sequence triggered without needing to press a key.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField]
        private DialogueInteractable interactable;

        [SerializeField]
        [Tooltip("List of conditions that need to be true for this trigger to work. Currently based on the name " +
            "of other triggers that have executed.")]
        private List<string> conditions;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (this.conditions != null)
            {
                if (!DialogueStates.Instance.CheckConditions(this.conditions))
                {
                    Debug.Log("Not all conditions are met, cannot start dialogue!");
                    return;
                }
            }

            this.interactable.OnInteract(collider.gameObject);
        }
    }
}
