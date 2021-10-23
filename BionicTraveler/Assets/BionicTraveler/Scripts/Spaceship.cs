namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Interaction;
    using UnityEngine;

    /// <summary>
    /// The player's spaceship, which allows her to travel to other planets and charge her battery.
    /// </summary>
    public class Spaceship : DialogueObject
    {
        /// <inheritdoc/>
        public override void OnInteract(GameObject obj)
        {
            obj.GetComponent<Player>()?.HealBattery();
            base.OnInteract(obj);
        }
    }
}
