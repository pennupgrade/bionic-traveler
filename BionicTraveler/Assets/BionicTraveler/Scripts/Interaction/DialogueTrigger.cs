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
        private void OnTriggerEnter2D(Collider2D collider)
        {
            this.interactable.OnInteract(collider.gameObject);
        }
    }
}
