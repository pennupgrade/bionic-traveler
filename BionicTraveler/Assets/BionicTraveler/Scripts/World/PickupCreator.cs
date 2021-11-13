namespace BionicTraveler.Scripts.World
{
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// Helper class to instantiate new <see cref="Pickup"/>s at runtime based on their prefab.
    /// </summary>
    public class PickupCreator : MonoBehaviour
    {
        [SerializeField]
        private Pickup prefab;

        /// <summary>
        /// Spawns a new pickup of <see cref="item"/> at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="item">The item.</param>
        /// <returns>The newly created pickup.</returns>
        public static Pickup SpawnPickup(Vector3 position, ItemData item)
        {
            var pickupCreator = FindObjectOfType<PickupCreator>();
            return pickupCreator.Spawn(position, item);
        }

        /// <summary>
        /// Spawns a new pickup of <see cref="item"/> at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="item">The item.</param>
        /// <returns>The newly created pickup.</returns>
        public Pickup Spawn(Vector3 position, ItemData item)
        {
            var newPickup = Instantiate(this.prefab, position, new Quaternion(0, 0, 0, 0));
            newPickup.SetItemData(item);
            return newPickup;
        }
    }
}