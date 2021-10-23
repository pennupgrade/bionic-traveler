namespace BionicTraveler.Scripts.Interaction
{
    using UnityEngine;

    /// <summary>
    /// Damages players that interact with it (mostly for testing purposes).
    /// </summary>
    public class InteractableDamager : MonoBehaviour, IInteractable
    {
        /// <inheritdoc/>
        public void OnInteract(GameObject obj)
        {
            obj.GetComponent<Player>()?.DamageBattery(1);
        }
    }
}
