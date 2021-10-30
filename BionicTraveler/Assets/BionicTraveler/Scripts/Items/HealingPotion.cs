namespace BionicTraveler.Scripts.Items
{
    using UnityEngine;

    /// <summary>
    /// A healing potion that heals the entity that uses it.
    /// </summary>
    [CreateAssetMenu(fileName = "HealingPotion", menuName = "Items/Consumables/Healing Potion")]
    public class HealingPotion : Consumable
    {
        [SerializeField]
        [Tooltip("The amount to heal the entity.")]
        private int healingEffect;

        /// <inheritdoc/>
        public override void Use(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}