namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// For stubbing only.
    /// </summary>
    [CreateAssetMenu(fileName = "Bodypart", menuName = "Items/Equippables/Bodypart")]
    public class Bodypart : Equippable
    {
        /// <inheritdoc/>
        public override void Equip(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Unequip(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
