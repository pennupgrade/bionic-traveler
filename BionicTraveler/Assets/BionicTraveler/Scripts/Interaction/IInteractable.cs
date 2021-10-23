namespace BionicTraveler.Scripts.Interaction
{
    using UnityEngine;

    /// <summary>
    /// Interface for objects that can be interacted with.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Perform some actions upon being interacted with.
        /// </summary>
        /// <param name="obj">Object which has interacted with this.</param>
        public void OnInteract(GameObject obj);
    }
}
