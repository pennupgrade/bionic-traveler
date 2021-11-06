namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// The player's spaceship, which allows her to travel to other planets and charge her battery.
    /// </summary>
    public class Spaceship : DialogueObject
    {
        /// <inheritdoc/>
        public override void OnInteract(GameObject obj)
        {
            Debug.Log("Interacted with Spaceship, healed Player");
            obj.GetComponent<PlayerEntity>()?.HealBattery();
            //base.OnInteract(obj);
        }
    }
}
