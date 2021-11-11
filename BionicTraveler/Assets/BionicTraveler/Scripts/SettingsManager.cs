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
    /// Drop down box changes int Difficulty, retrieved with GetDifficulty()
    ///     0 : Peaceful
    ///     1 : Adventure
    ///     2 : Hell
    /// </summary>
    public class SettingsManager : Menu
    {
        [SerializeField]
        private AudioMixer bgmMixer; // AudioMixers to be dragged in

        [SerializeField]
        private AudioMixer sfxMixer;
        [SerializeField]
        private AudioMixer voiceMixer;
        private float masterVolume;
        private bool masterOn;

        private float bgmVolume;
        private bool bgmOn;

        private float sfxVolume;
        private bool sfxOn;

        private float voiceVolume;
        private bool voiceOn;

        private int difficulty;

        /// <summary>
        /// Gets singleton
        /// </summary>
        public static SettingsManager Instance { get; private set; }

        /// <summary>
        /// Returns difficulty
        /// 0 : Peaceful
        /// 1 : Adventure
        /// 2 : Hell
        /// </summary>
        /// <returns>game difficulty</returns>
        public int GetDifficulty()
        {
            return this.difficulty;
        }

        private void SetDifficulty(int diff)
        {
            this.difficulty = diff;
        }

        private void SetMaster(float masterSlider)
        {
            this.masterVolume = masterSlider;
            this.LogScaling(this.bgmMixer, this.masterVolume, this.masterOn, "bgmVolume", this.bgmVolume, this.bgmOn);
            this.LogScaling(this.sfxMixer, this.masterVolume, this.masterOn, "sfxVolume", this.sfxVolume, this.sfxOn);
            this.LogScaling(this.voiceMixer, this.masterVolume, this.masterOn, "voiceVolume", this.voiceVolume, this.voiceOn);
        }

        private void MuteMaster(bool muteMaster)
        {
            this.masterOn = muteMaster;
            this.LogScaling(this.bgmMixer, this.masterVolume, this.masterOn, "bgmVolume", this.bgmVolume, this.bgmOn);
            this.LogScaling(this.sfxMixer, this.masterVolume, this.masterOn, "sfxVolume", this.sfxVolume, this.sfxOn);
            this.LogScaling(this.voiceMixer, this.masterVolume, this.masterOn, "voiceVolume", this.voiceVolume, this.voiceOn);
        }

        private void SetBgm(float bgmSlider)
        {
            this.bgmVolume = bgmSlider;
            this.LogScaling(this.bgmMixer, this.masterVolume, this.masterOn, "bgmVolume", this.bgmVolume, this.bgmOn);
        }

        private void MuteBgm(bool muteBgm)
        {
            this.bgmOn = muteBgm;
            this.LogScaling(this.bgmMixer, this.masterVolume, this.masterOn, "bgmVolume", this.bgmVolume, this.bgmOn);
        }

        private void SetSfx(float sfxSlider)
        {
            this.sfxVolume = sfxSlider;
            this.LogScaling(this.sfxMixer, this.masterVolume, this.masterOn, "sfxVolume", this.sfxVolume, this.sfxOn);
        }

        private void MuteSfx(bool muteSfx)
        {
            this.sfxOn = muteSfx;
            this.LogScaling(this.sfxMixer, this.masterVolume, this.masterOn, "sfxVolume", this.sfxVolume, this.sfxOn);
        }

        private void SetVoice(float voiceSlider)
        {
            this.voiceVolume = voiceSlider;
            this.LogScaling(this.voiceMixer, this.masterVolume, this.masterOn, "voiceVolume", this.voiceVolume, this.voiceOn);
        }

        private void MuteVoice(bool muteVoice)
        {
            this.voiceOn = muteVoice;
            this.LogScaling(this.voiceMixer, this.masterVolume, this.masterOn, "voiceVolume", this.voiceVolume, this.voiceOn);
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Converts float values on each of the sliders to a decibel level on each of the AudioMixers
        /// If the mixer and master toggles are both on
        ///     Reads the mixer and master sliders, converting the product to a log scale from 0 to -80
        /// Otherwise mute the channel (set volume to -80dB)
        /// </summary>
        /// <param name="mixerName">name of the AudioMixer</param>
        /// <param name="masterVal">float value of master slider</param>
        /// <param name="isMaster">boolean true if master toggle is checked</param>
        /// <param name="volumeLabel">name of volume exposed volume field</param>
        /// <param name="sliderVal">float value of mixer slider</param>
        /// <param name="isOn">boolean true if mixer toggle is checked</param>
        /// Eg.
        /// mixerName = 'bgmMixer'
        /// masterVal = 1.0f
        /// isMaster = masterOn = true
        /// volumeLabel = 'bgmVolume'
        /// sliderVal = 0.5f
        /// isOn = bgmOn = true
        ///
        /// The exposed float 'bgmVolume' on the AudioMixer 'bgmMixer' will be set to log10(1.0 * 0.5) = -6.0f
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
    }
}
