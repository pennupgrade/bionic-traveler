using BionicTraveler.Scripts.World;

namespace BionicTraveler.Scripts.Items
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A energy potion that adds energy to the entity that uses it.
    /// </summary>
    [CreateAssetMenu(fileName = "EnergyPotion", menuName = "Items/Consumables/Energy Potion")]
    public class EnergyPotion : Consumable
    {
        [SerializeField]
        [Tooltip("The amount of energy to add to the entity.")]
        private int energyEffect;
        
        /// <inheritdoc/>
        public override void Use(Entity entity)
        {
            if (entity.IsDynamic)
            {
                DynamicEntity dynamicEntity = entity.GetComponent<DynamicEntity>();
                Debug.Log($"Energy Prior: {dynamicEntity.Energy}");
                dynamicEntity.AddEnergy(this.energyEffect);
                Debug.Log($"{this.energyEffect} energy added; New Energy: {dynamicEntity.Energy}");
            }
            else
            {
                throw new Exception($"{entity.name} is not dynamic");
            }
        }
    }
}
