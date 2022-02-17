using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace BionicTraveler.Scripts.Audio
{
    using System.Collections.Generic;
    using Framework;
    using UnityEngine;
    using UnityEngine.SceneManagement;

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

        /*[Tooltip("The master volume of all sounds.")]
        [Range(0.0f, 1.0f)]
        public float MasterVolume;*/

        [Tooltip("The volume of all effect sounds.")]
        [Range(0.0f, 1.0f)]
        public float EffectsVolume;

        [Tooltip("The volume of the background music.")]
        [Range(0.0f, 1.0f)]
        public float MusicVolume;

        [Tooltip("The volume of the dialogue voice.")]
        [Range(0.0f, 1.0f)]
        public float VoiceVolume = 0.5f;

        [SerializeField]
        [Tooltip("The list of possible background songs.")]
        private List<BackgroundClip> backgroundClips;

        [SerializeField]
        [Tooltip("The main audio source.")]
        private AudioSource mainAudioSource;

        [SerializeField]
        [Tooltip("The background audio source.")]
        private AudioSource backgroundSource;

        [SerializeField]
        [Tooltip("The upcoming background audio source. (used to crossfade between songs and stuff)")]
        private AudioSource upcomingBackgroundSource;

        private BackgroundClip currentBackgroundClip;
        private BackgroundClip upcomingBackgroundClip;

        [SerializeField]
        [Tooltip("The dialog audio source.")]
        private AudioSource dialogueSource;

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
        /// Plays the dialogue clip.
        /// </summary>
        public void PlayDialogueSound() {
            // TODO: Instead of using a static audio clip, set up metadata for a dialgoue interactable to choose between our dialogue sounds
            dialogueSource.Play();
        }

        /// <summary>
        /// Stops the dialogue clip.
        /// </summary>
        public void StopDialogueSound() {
            dialogueSource.Stop();
        }

        /// <summary>
        /// Plays <see cref="clip"/>. Existing playback will be stopped.
        /// </summary>
        /// <param name="clip">The clip.</param>
        public void PlayOneShot(AudioClip clip)
        {
            this.mainAudioSource.PlayOneShot(clip, this.EffectsVolume);
        }


        /// <summary>
        /// Starts the background music playback.
        /// </summary>
        public void StartBackgroundMusic()
        {
            string scene = SceneManager.GetActiveScene().name;
            Debug.Log(scene);

            foreach (BackgroundClip s in backgroundClips)
            {
                if (s.Scenes.Contains(scene))
                {
                    upcomingBackgroundClip = s;
                    currentBackgroundClip = upcomingBackgroundClip;

                    backgroundSource.clip = currentBackgroundClip.Clip;
                    backgroundSource.Play();
                }
            }
            
          }

        private void PlayUpcomingBackgroundClip() {
            
            // exchange which of the audio sources is the main
            AudioSource temp = this.backgroundSource;
            this.backgroundSource = this.upcomingBackgroundSource;
            this.upcomingBackgroundSource = temp;

            // exchange the metadata object
            this.currentBackgroundClip = this.upcomingBackgroundClip;

            // actually play the clip
            this.backgroundSource.clip = this.currentBackgroundClip.Clip;

            if(currentBackgroundClip == upcomingBackgroundClip)
            {
                this.backgroundSource.time = upcomingBackgroundClip.LoopStart;
            }

            this.backgroundSource.Play();

            Debug.Log($"Playing next background clip {this.backgroundSource.clip.name}");

        }

        /// <summary>
        /// Pauses the background music playback.
        /// </summary>
        public void PauseBackgroundMusic()
        {
            this.backgroundSource.Pause();
        }

        /// <summary>
        /// Resumes the background music playback.
        /// </summary>
        public void ResumeBackgroundMusic()
        {
            this.backgroundSource.UnPause();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.UpdateVolume();
            if (this.backgroundSource.playOnAwake)
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
            if (Application.isFocused && !this.backgroundSource.isPlaying)
            {
                this.backgroundSource.Play();
            }

            // Ensure we do not crash if nothing is playing right now.
            if (this.currentBackgroundClip != null)
            {
                if (this.backgroundSource.time >= this.currentBackgroundClip.LoopPoint)
                {
                    this.PlayUpcomingBackgroundClip();
                }
            }
        }

        /// <summary>
        /// Updates volume of AudioSources according to floats.
        /// </summary>
        public void UpdateVolume()
        {
            this.mainAudioSource.volume = this.EffectsVolume;
            this.dialogueSource.volume = this.VoiceVolume;
            this.backgroundSource.volume = this.MusicVolume;
            this.upcomingBackgroundSource.volume = this.MusicVolume;
        }
    }
}
