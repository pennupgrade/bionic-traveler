namespace BionicTraveler
{
    using UnityEngine;

    /// <summary>
    /// Script to achieve game object persistence.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.transform.gameObject);
        }
    }
}