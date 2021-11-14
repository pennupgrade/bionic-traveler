namespace BionicTraveler.Scripts.Items
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Table of items.
    /// </summary>
    [CreateAssetMenu(fileName ="New Loot Table", menuName = "Items/LootTable")]
    public class LootTable : ScriptableObject
    {
        [SerializeField]
        [Tooltip("List of items to drop.")]
        private List<ItemData> items;

        /// <summary>
        /// Gets the items.
        /// </summary>
        public List<ItemData> Items => this.items;
    }
}
