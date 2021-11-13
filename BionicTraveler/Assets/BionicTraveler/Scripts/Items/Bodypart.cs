namespace BionicTraveler.Scripts.Items
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes a bodypart slot.
    /// </summary>
    public enum BodypartSlot
    {
        /// <summary>
        /// The right arm.
        /// </summary>
        RightArm,

        /// <summary>
        /// The left arm.
        /// </summary>
        LeftArm,

        /// <summary>
        /// The legs.
        /// </summary>
        Legs,
    }

    /// <summary>
    /// The base class for all bodyparts.
    /// </summary>
    public abstract class Bodypart : Equippable
    {
        protected DynamicEntity Owner { get; private set; }

        /// <summary>
        /// Gets the player.
        /// </summary>
        public DynamicEntity Player => this.Owner;

        /// <summary>
        /// Gets the slot this bodypart can be equipped in.
        /// </summary>
        public abstract BodypartSlot Slot { get; }

        /// <summary>
        /// Activates the bodypart's ability.
        /// </summary>
        public abstract void ActivateAbility();

        /// <inheritdoc/>
        public override void Equip(DynamicEntity entity)
        {
            // TODO: For now, we immediately set it as a bodypart.
            var bodypartBehavior = entity.gameObject.GetComponent<BodypartBehaviour>();
            if (bodypartBehavior != null && bodypartBehavior.EquipBodypart(this))
            {
                this.Owner = entity ?? throw new System.ArgumentNullException(nameof(entity));
            }
        }

        /// <inheritdoc/>
        public override void Unequip(DynamicEntity entity)
        {
            this.Owner = null;
        }
    }
}
