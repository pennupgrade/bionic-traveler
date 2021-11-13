namespace BionicTraveler
{
    using UnityEngine;

    /// <summary>
    /// Main game controller for persistence.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.transform.gameObject); // set to dont destroy
        }
    }
}