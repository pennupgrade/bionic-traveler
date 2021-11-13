namespace BionicTraveler.Scripts.Interaction
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Controls player interactions with the world.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField]
        private float interactionRadius = 1f;

        private void Update()
        {
            if (Input.GetButtonDown("Interact"))
            {
                this.CheckForInteraction();
            }
        }

        private void CheckForInteraction()
        {
            var position = this.transform.position;
            var hits = new List<RaycastHit2D>();
            var contactFilter = default(ContactFilter2D);
            int resultsNo = Physics2D.CircleCast(position, this.interactionRadius, Vector2.zero, contactFilter, hits);

            if (resultsNo > 0)
            {
                foreach (var result in hits)
                {
                    if (result.collider != null)
                    {
                        this.Interact(result.collider.gameObject);
                    }
                }
            }
        }

        private void Interact(GameObject obj)
        {
            obj.GetComponent<IInteractable>()?.OnInteract(this.gameObject);
        }
    }
}
