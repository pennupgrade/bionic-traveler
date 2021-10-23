namespace BionicTraveler.Scripts.Interaction
{
    using UnityEngine;

    /// <summary>
    /// Controls player interactions with the world.
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField]
        private float interactionRadius = 1f;

        private void FixedUpdate()
        {
            if (Input.GetAxis("Interact") > 0)
            {
                this.CheckForInteraction();
            }
        }

        private void CheckForInteraction()
        {
            var position = this.transform.position;
            RaycastHit2D hit = Physics2D.CircleCast(position, this.interactionRadius, Vector2.zero);

            if (hit.collider != null)
            {
                this.Interact(hit.collider.gameObject);
            }
        }

        private void Interact(GameObject obj)
        {
            obj.GetComponent<IInteractable>()?.OnInteract(this.gameObject);
        }
    }
}
