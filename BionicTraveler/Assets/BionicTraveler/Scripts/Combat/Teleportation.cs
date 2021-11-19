namespace BionicTraveler.Scripts.Combat
{
    using System.Linq;
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Implements the player's teleportation ability.
    /// </summary>
    [CreateAssetMenu(fileName = "Teleportation", menuName = "Attacks/Teleportation")]
    public class Teleportation : Bodypart
    {
        [SerializeField]
        [Tooltip("The distance the teleport can cover maximally.")]
        private float distance;

        /// <inheritdoc/>
        public override BodypartSlot Slot => BodypartSlot.RightArm;

        /// <inheritdoc/>
        public override void ActivateAbility()
        {
            var ourPos = this.Owner.gameObject.transform.position;
            var finalPosition = this.GetTeleportEndPosition(ourPos, this.Owner.Direction, this.distance);
            this.Owner.gameObject.transform.position = finalPosition;
        }

        private Vector3 GetTeleportEndPosition(Vector3 startPosition, Vector3 direction, float remainingDistance)
        {
            var hits = Physics2D.RaycastAll(startPosition, direction, remainingDistance);
            var sortedHits = hits.OrderBy(hit => Vector2.Distance(startPosition, hit.point))
                .Where(hit => hit.collider != null).ToArray();

            foreach (var hit in sortedHits)
            {
                // Check if collider is traversable.
                if (hit.collider.gameObject.GetComponent<TilemapCollider2D>())
                {
                    if (!hit.collider.CompareTag("TraversableCollider"))
                    {
                        // By default, we are not able to traverse tilemap colliders.
                        // To make sure that we do not get nudged to the wrong side by the
                        // Unity physics engine we make sure we get placed a bit before the actual hit.
                        // Unfortunately, this is necessary as otherwise the engine sometimes glitches
                        // the player through the wall.
                        return hit.point + (hit.normal * 1.0f);
                    }
                }
            }

            return startPosition + (this.Owner.Direction * this.distance);
        }
    }
}
