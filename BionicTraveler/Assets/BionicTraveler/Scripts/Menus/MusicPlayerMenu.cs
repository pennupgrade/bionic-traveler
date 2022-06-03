namespace BionicTraveler.Scripts.Menus
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The main music player menu.
    /// </summary>
    public class MusicPlayerMenu : Menu
    {
        [SerializeField]
        private AudioClip[] clips;

        [SerializeField]
        private Dropdown songDropdown;

        [SerializeField]
        private Slider volumeSlider;

        [SerializeField]
        private AudioSource audioSource;

        /// <inheritdoc/>
        public override void Start()
        {
            base.Start();
            this.songDropdown.ClearOptions();
            this.songDropdown.AddOptions(this.clips.Select(c => c.name).ToList());
            this.volumeSlider.onValueChanged.AddListener(f => this.audioSource.volume = f);
        }

        /// <summary>
        /// Plays the currently selected song.
        /// </summary>
        public void Play()
        {
            this.audioSource.Stop();
            this.audioSource.volume = this.volumeSlider.value;
            this.audioSource.clip = this.clips[this.songDropdown.value];
            this.audioSource.Play();
        }

        /// <summary>
        /// Stops the currently selected song.
        /// </summary>
        public void Stop()
        {
            this.audioSource.Stop();
        }

        /// <summary>
        /// Properly closes the current menu.
        /// </summary>
        public void CloseMenu()
        {
            WindowManager.Instance.ToggleMenu(this);
        }
    }
}