namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Audio;
    using UnityEngine;

    /// <summary>
    /// Interface that all menus must implement. Contains an open and close method.
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        private Canvas canvas; // Canvas containing the menu

        [SerializeField]
        private AudioClip openSound;
        [SerializeField]
        private AudioClip closeSound;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public virtual void Start()
        {
            this.canvas = this.GetComponent<Canvas>();
            this.Close();
        }

        /// <summary>
        /// Opens the menu canvas and pauses the game.
        /// </summary>
        public virtual void Open()
        {
            AudioManager.Instance.PlayOneShot(this.openSound);
            this.canvas.enabled = true;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Closes the menu canvas and resumes the game.
        /// </summary>
        public virtual void Close()
        {
            AudioManager.Instance.PlayOneShot(this.closeSound);
            this.canvas.enabled = false;
            Time.timeScale = 1;
        }
    }
}
