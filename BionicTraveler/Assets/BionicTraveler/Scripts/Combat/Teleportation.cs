namespace BionicTraveler.Scripts.Combat
{
    using System.Linq;
    using BionicTraveler.Scripts.Items;
    using UnityEngine;
    using UnityEngine.Tilemaps;
    using System.Collections;
    using BionicTraveler.Scripts.World;

    /// <summary>
    /// Implements the player's teleportation ability.
    /// </summary>
    [CreateAssetMenu(fileName = "Teleportation", menuName = "Attacks/Teleportation")]
    public class Teleportation : Bodypart
    {
        [SerializeField]
        [Tooltip("The distance the teleport can cover maximally.")]
        private float distance;

        [SerializeField]
        [Tooltip("The energy the teleport consumes for full distance. If low an energy, a partial teleport" +
            "is performed.")]
        private int energyCost;

        [SerializeField]
        private int phase;

        private Vector3 targetPosition;

        /// <inheritdoc/>
        public override BodypartSlot Slot => BodypartSlot.RightArm;

        /// <summary>
        /// Changes the actual position of the owner of this teleport
        /// body part to the target position. Called dynamically from
        /// <see cref="BodypartBehaviour"/>.
        /// </summary>
        public void MoveEntity() 
        {
            this.Owner.gameObject.transform.position = targetPosition;
        }

        /// <summary>
        /// Unlocked player control after the animation has 
        /// finished playing. Called dynamically from
        /// <see cref="BodypartBehaviour"/>.
        /// </summary>
        public void OnAnimationFinished()
        {
            if (this.Owner is PlayerEntity playerEntity)
            {
                playerEntity.EnableInput();
            }
        }

        /// <inheritdoc/>
        public override void ActivateAbility()
        {
            // Scale the distance by the energy we have. If we have less than necessary, we move less.
            var ourPos = this.Owner.gameObject.transform.position;
            var energyToConsume = Mathf.Min(this.Owner.Energy, this.energyCost);
            var distanceMultiplier = (energyToConsume / this.energyCost) * 4;
            var finalDistance = this.distance * distanceMultiplier;
            var animator = this.Owner.GetComponent<Animator>();

            // Get teleport target position. We do not allow positions behind us.
            // This could happen due to bad raytracing when objects are directly in front of us.
            var finalPosition = this.GetTeleportEndPosition(ourPos, this.Owner.Direction, finalDistance);
            if (!this.Owner.IsAheadOf(finalPosition))
            {
                // Lock all functionality here.
                if (this.Owner is PlayerEntity playerEntity)
                {
                    playerEntity.DisableInput();
                }

                animator.Play("Disappear");
                this.targetPosition = finalPosition;
                this.Owner.RemoveEnergy(this.energyCost);
            }
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

            return startPosition + (this.Owner.Direction * remainingDistance);
        }
    }
}
