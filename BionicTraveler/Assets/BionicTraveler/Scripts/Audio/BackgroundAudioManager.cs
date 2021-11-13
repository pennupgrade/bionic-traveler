namespace BionicTraveler.Scripts.Audio
{
    using System.Collections.Generic;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Represents background audio.
    /// </summary>
    public class BackgroundAudioManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of possible background songs.")]
        private List<AudioClip> clips;

        [SerializeField]
        [Tooltip("The audio source.")]
        private AudioSource audioSource;
        private bool hasStarted;

        /// <summary>
        /// Starts the background music playback.
        /// </summary>
        public void StartPlayback()
        {
            this.PlayRandomClip();
        }

        private void PlayRandomClip()
        {
            this.audioSource.clip = this.GetRandomClip();
            this.audioSource.Play();
            this.hasStarted = true;
            Debug.Log($"Playing next background clip {this.audioSource.clip.name}");
        }

        private AudioClip GetRandomClip()
        {
            return this.clips.GetRandomItem();
        }

        /// <summary>
        /// Pauses the background music playback.
        /// </summary>
        public void PausePlayback()
        {
            this.audioSource.Pause();
        }

        /// <summary>
        /// Resumes the background music playback.
        /// </summary>
        public void ResumePlayback()
        {
            this.audioSource.UnPause();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            if (this.audioSource.playOnAwake)
            {
                this.StartPlayback();
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (this.hasStarted && !this.audioSource.isPlaying)
            {
                this.PlayRandomClip();
            }
        }
    }
}
