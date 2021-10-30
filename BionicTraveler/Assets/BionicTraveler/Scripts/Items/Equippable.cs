namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;

    /// <summary>
    /// The base class for all equippable items, e.g. body parts. The Unity inspector does not play nice with
    /// interfaces so we use a class here.
    /// </summary>
    public abstract class Equippable : ScriptableObject
    {
        /// <summary>
        /// Equips an equippable item.
        /// </summary>
        /// <param name="entity">The entity that equips the item.</param>
        public abstract void Equip(Entity entity);

        /// <summary>
        /// Unequips an equippable item.
        /// </summary>
        /// <param name="entity">The entity that unequips the item.</param>
        public abstract void Unequip(Entity entity);
    }
}
