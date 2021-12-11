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
        private AudioClip clickSound;

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
            AudioManager.Instance.PlayOneShot(clickSound);
            this.difficulty = diff;
        }

        private void SetMaster(float masterSlider)
        {
            this.masterVolume = masterSlider;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void MuteMaster(bool muteMaster)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.masterOn = muteMaster;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void SetBgm(float bgmSlider)
        {
            this.bgmVolume = bgmSlider;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void MuteBgm(bool muteBgm)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.bgmOn = muteBgm;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void SetSfx(float sfxSlider)
        {
            this.sfxVolume = sfxSlider;
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void MuteSfx(bool muteSfx)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.sfxOn = muteSfx;
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
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
