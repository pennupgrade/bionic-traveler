namespace BionicTraveler.Scripts.Interaction
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// A dialogue sequence triggered without needing to press a key.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField]
        private DialogueInteractable interactable;

        [Tooltip("Whether AI can trigger this dialogue.")]
        [SerializeField]
        private bool canBeTriggeredByAI;

        [SerializeField]
        [Tooltip("List of conditions that need to be true for this trigger to work. Currently based on the name " +
            "of other triggers that have executed.")]
        private List<string> conditions;

        private bool hasTriggered;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            this.Check(collider);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            this.Check(collision);
        }

        private void Check(Collider2D collider)
        {
            if (this.hasTriggered)
            {
                return;
            }

            if (this.conditions != null)
            {
                if (!DialogueStates.Instance.CheckConditions(this.conditions))
                {
                    //Debug.Log("Not all conditions are met, cannot start dialogue!");
                    return;
                }
            }

            var collidedGameobject = collider.gameObject;
            var entity = collidedGameobject.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.IsPlayer || (this.canBeTriggeredByAI && entity.IsDynamic))
                {
                    this.interactable.OnInteract(collider.gameObject);
                    this.hasTriggered = true;
                }
            }
        }
    }
}
