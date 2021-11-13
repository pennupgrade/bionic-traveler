namespace BionicTraveler.Scripts.Audio
{
    using System.Collections.Generic;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Represents background audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        /// <summary>
        /// Gets the audio manager instance.
        /// </summary>
        public static AudioManager Instance => AudioManager.instance;

        [SerializeField]
        [Tooltip("The list of possible background songs.")]
        private List<AudioClip> backgroundClips;

        [SerializeField]
        [Tooltip("The master volume of all sounds.")]
        [Range(0.0f, 1.0f)]
        private float masterVolume;

        [SerializeField]
        [Tooltip("The volume of all effect sounds.")]
        [Range(0.0f, 1.0f)]
        private float effectsVolume;

        [SerializeField]
        [Tooltip("The audio source.")]
        private AudioSource audioSource;
        private bool hasStarted;

        private void Awake()
        {
            if (AudioManager.instance != null && AudioManager.instance != this)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else
            {
                AudioManager.instance = this;
            }
        }

        /// <summary>
        /// Plays <see cref="clip"/>. Existing playback will be stopped.
        /// </summary>
        /// <param name="clip">The clip.</param>
        public void PlayOneShot(AudioClip clip)
        {
            this.audioSource.PlayOneShot(clip, this.masterVolume * this.effectsVolume);
        }

        /// <summary>
        /// Starts the background music playback.
        /// </summary>
        public void StartBackgroundMusic()
        {
            this.PlayRandomBackgroundClip();
        }

        private void PlayRandomBackgroundClip()
        {
            this.audioSource.clip = this.GetRandomBackgroundClip();
            this.audioSource.Play();
            this.hasStarted = true;
            Debug.Log($"Playing next background clip {this.audioSource.clip.name}");
        }

        private AudioClip GetRandomBackgroundClip()
        {
            return this.backgroundClips.GetRandomItem();
        }

        /// <summary>
        /// Pauses the background music playback.
        /// </summary>
        public void PauseBackgroundMusic()
        {
            this.audioSource.Pause();
        }

        /// <summary>
        /// Resumes the background music playback.
        /// </summary>
        public void ResumeBackgroundMusic()
        {
            this.audioSource.UnPause();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.UpdateVolume();
            if (this.audioSource.playOnAwake)
            {
                this.StartBackgroundMusic();
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            // Audio stops when we are not focused, which would then trigger a new song whenever we tab out.
            if (this.hasStarted && Application.isFocused && !this.audioSource.isPlaying)
            {
                this.PlayRandomBackgroundClip();
            }
        }



        private void OnValidate()
        {
            this.UpdateVolume();
        }

        private void UpdateVolume()
        {
            this.audioSource.volume = this.masterVolume;
        }
    }
}
