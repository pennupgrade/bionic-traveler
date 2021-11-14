namespace BionicTraveler.Scripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Transitions into a new scene on trigger.
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField]
        private string sceneToLoad;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                SceneManager.LoadScene(this.sceneToLoad);
            }
        }
    }
}