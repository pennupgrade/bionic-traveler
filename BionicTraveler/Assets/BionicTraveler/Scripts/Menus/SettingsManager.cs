using System.Diagnostics;
using BionicTraveler.Scripts.Audio;
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
        private AudioManager audioManager;

        private float masterVolume = 0.5f;
        private bool masterOn = true;

        private float bgmVolume = 0.5f;
        private bool bgmOn = true;

        private float sfxVolume = 0.5f;
        private bool sfxOn = true;

        /*private float voiceVolume;
        private bool voiceOn;*/

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
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteMaster(bool muteMaster)
        {
            this.masterOn = muteMaster;
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void SetBgm(float bgmSlider)
        {
            this.bgmVolume = bgmSlider;
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteBgm(bool muteBgm)
        {
            this.bgmOn = muteBgm;
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.UpdateVolume();
        }

        private void SetSfx(float sfxSlider)
        {
            this.sfxVolume = sfxSlider;
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteSfx(bool muteSfx)
        {
            this.sfxOn = muteSfx;
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        /* Voices not implemented yet, will fill in later
        private void SetVoice(float voiceSlider)
        {
            this.voiceVolume = voiceSlider;
            audioManager.VoiceVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.voiceVolume, this.voiceOn);
        }

        private void MuteVoice(bool muteVoice)
        {
            this.voiceOn = muteVoice;
            audioManager.VoiceVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.voiceVolume, this.voiceOn);
        }
        */

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

        private float ChangeVolume(float masterVal, bool isMaster, float sliderVal, bool isOn)
        {
            if (isOn && isMaster)
            {
                return sliderVal * masterVal;
            }
            else
            {
                return 0f;
            }
        }
    }
}
