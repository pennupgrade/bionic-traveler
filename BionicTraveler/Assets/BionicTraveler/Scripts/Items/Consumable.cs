namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;

    /// <summary>
    /// The base class for all consumable items, e.g. health potions.
    /// </summary>
    public abstract class Consumable : ScriptableObject
    {
        /// <summary>
        /// Uses a consumable item.
        /// </summary>
        /// <param name="entity">The entity that consumes the item.</param>
        public abstract void Use(Entity entity);
    }
}
