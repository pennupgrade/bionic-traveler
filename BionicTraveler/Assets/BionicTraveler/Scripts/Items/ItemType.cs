namespace BionicTraveler.Scripts.Items
{
    /// <summary>
    /// Describes the type of an item.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// An unspecified item type.
        /// </summary>
        None,

        /// <summary>
        /// A consumable item, e.g. a health potion.
        /// </summary>
        Consumable,

        /// <summary>
        /// An equippable item, e.g. a bodypart.
        /// </summary>
        Equippable,
    }
}