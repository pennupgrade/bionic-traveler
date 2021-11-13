namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;

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
            var targetPos = ourPos + (this.Owner.Direction * this.distance);
            this.Owner.transform.position = targetPos;
        }
    }
}
