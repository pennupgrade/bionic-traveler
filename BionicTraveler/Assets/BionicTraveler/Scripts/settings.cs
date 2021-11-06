using UnityEngine.Audio;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Script should be attached to the GameObject 'SettingsMenu'
    /// GameObject 'SettingsMenu' should be dragged in as the SerializedField 'canvas'
    /// Three AudioMixers can be dragged in, corresponding to bgm, sfx, and voice named bgmMixer, sfxMixer, voiceMixer respectively
    /// Menu opened with 'Esc'
    /// Toggle buttons and sliders change the volume on the corresponding AudioMixer from 0dB to -80dB
    ///     Logarithmic curve
    ///     100% volume :   0dB
    ///      50% volume :  -6dB
    ///       0% volume : -80dB
    ///     AudioMixer will be -80dB (muted) unless both the master toggle and corresponding toggle are checked
    /// Drop down box changes int Difficulty
    ///     0 : Peaceful
    ///     1 : Adventure
    ///     2 : Hell
    /// </summary>

    public class Settings : MonoBehaviour
    {
        public static Settings Instance { get; private set; }
        private bool open; // True if the menu is open
        
        [SerializeField]
        private Canvas canvas; // Canvas containing the menu
        
        [SerializeField]
        private AudioMixer bgmMixer, sfxMixer, voiceMixer; //AudioMixers to be dragged in

        private float masterVolume;
        private bool masterOn;

        private float bgmVolume;
        private bool bgmOn;

        private float sfxVolume;
        private bool sfxOn;
        
        private float voiceVolume;
        private bool voiceOn;
        
        public int Difficulty;

        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(this.gameObject);
            } else {
                Instance = this;
            }
        }

        void Start() // Menu is closed upon game launch
        {
            Close();
        }

        public void Update() // Listens for 'Esc' key press which will open the menu if it is closed and vice versa
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (open)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }

        public void Open() // Opens the SettingsMenu canvas and pauses the game
        {
            open = true;
            canvas.enabled = true;
            Time.timeScale = 0;
        }

        public void Close() // Closes the SettingsMenu canvas and resumes the game
        {
            open = false;
            canvas.enabled = false;
            Time.timeScale = 1;
        }
        
        public void SetDifficulty(int diff) // Sets Difficulty equal to the drop down box
        {
            Difficulty = diff;
        }
        
        /// <summary>
        /// Converts float values on each of the sliders to a decibel level on each of the AudioMixers
        /// If the mixer and master toggles are both on
        ///     Reads the mixer and master sliders, converting the product to a log scale from 0 to -80
        /// Otherwise mute the channel (set volume to -80dB)
        /// </summary>
        /// <param name="mixerName"></param> name of the AudioMixer
        /// <param name="masterVal"></param> float value of master slider
        /// <param name="isMaster"></param> boolean true if master toggle is checked
        /// <param name="volumeLabel"></param> name of volume exposed volume field
        /// <param name="sliderVal"></param> float value of mixer slider
        /// <param name="isOn"></param> boolean true if mixer toggle is checked
        /// Eg.
        ///     mixerName = 'bgmMixer'
        ///     masterVal = 1.0f
        ///     isMaster = masterOn = true
        ///     volumeLabel = 'bgmVolume'
        ///     sliderVal = 0.5f
        ///     isOn = bgmOn = true
        ///
        ///     The exposed float 'bgmVolume' on the AudioMixer 'bgmMixer' will be set to log10(1.0 * 0.5) = -6.0f
        private void LogScaling(AudioMixer mixerName, float masterVal, bool isMaster, string volumeLabel, float sliderVal, bool isOn)
        {
            if (isOn && isMaster)
            {
                mixerName.SetFloat(volumeLabel, Mathf.Log10(Mathf.Max(sliderVal * masterVal, 0.0001f)) * 20);
                
            }
            else
            {
                mixerName.SetFloat(volumeLabel, -80f);
            }
        }

        public void SetMaster(float masterSlider) // Updates all mixer volumes when the master slider is changed
        {
            masterVolume = masterSlider;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
            }

        public void MuteMaster(bool muteMaster) // Updates all mixer volumes when the master toggle is changed
        {
            masterOn = muteMaster;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }
        
        public void SetBgm(float bgmSlider) // Updates only the bgm mixer when bgm slider is changed
        {
            bgmVolume = bgmSlider;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
        }

        public void MuteBgm(bool muteBgm) // Updates only the bgm mixer when bgm toggle is changed
        {
            bgmOn = muteBgm;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
        }
        
        public void SetSfx(float sfxSlider) // Updates only the sfx mixer when sfx slider is changed
        {
            sfxVolume = sfxSlider;
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
        }
        
        public void MuteSfx(bool muteSfx) // Updates only the sfx mixer when sfx toggle is changed
        {
            sfxOn = muteSfx;
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
        }

        public void SetVoice(float voiceSlider) // Updates only the voice mixer when voice slider is changed
        {
            voiceVolume = voiceSlider;
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }

        public void MuteVoice(bool muteVoice) // Updates only the voice mixer when voice toggle is changed
        {
            voiceOn = muteVoice;
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }
    }
}
