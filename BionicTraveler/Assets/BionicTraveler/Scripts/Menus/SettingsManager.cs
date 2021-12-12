using System.Diagnostics;
using BionicTraveler.Scripts.Audio;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Script should be attached to the GameObject 'SettingsMenu'
    /// GameObject 'SettingsMenu' should be dragged in as the SerializedField 'canvas'
    /// </summary>
    public class SettingsManager : Menu
    {
        [SerializeField]
        private AudioManager audioManager;
        [SerializeField]
        private UnityEngine.UI.Slider masterS;
        [SerializeField]
        private UnityEngine.UI.Slider bgmS;
        [SerializeField]
        private UnityEngine.UI.Slider sfxS;
        [SerializeField]
        private UnityEngine.UI.Toggle masterT;
        [SerializeField]
        private UnityEngine.UI.Toggle bgmT;
        [SerializeField]
        private UnityEngine.UI.Toggle sfxT;

        [SerializeField]
        private float masterVolume;
        [SerializeField]
        private bool masterOn = true;

        private float bgmVolume;
        private bool bgmOn = true;

        private float sfxVolume;
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
            PlayerPrefs.SetFloat("masterVolume", this.masterVolume);
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteMaster(bool muteMaster)
        {
            this.masterOn = muteMaster;
            PlayerPrefs.SetInt("masterOn", this.BoolToInt(this.masterOn));
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void SetBgm(float bgmSlider)
        {
            this.bgmVolume = bgmSlider;
            PlayerPrefs.SetFloat("bgmVolume", this.bgmVolume);
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteBgm(bool muteBgm)
        {
            this.bgmOn = muteBgm;
            PlayerPrefs.SetInt("bgmOn", this.BoolToInt(this.bgmOn));
            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.UpdateVolume();
        }

        private void SetSfx(float sfxSlider)
        {
            this.sfxVolume = sfxSlider;
            PlayerPrefs.SetFloat("sfxVolume", this.sfxVolume);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        private void MuteSfx(bool muteSfx)
        {
            this.sfxOn = muteSfx;
            PlayerPrefs.SetInt("sfxOn", this.BoolToInt(this.sfxOn));
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

            this.LoadAll();

            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }

        /*public override void Start()
        {
            this.LoadAll();

            this.audioManager.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            this.audioManager.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            this.audioManager.UpdateVolume();
        }*/

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

        private int BoolToInt(bool b)
        {
            if (b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private bool IntToBool(int i)
        {
            return i != 0;
        }

        private void LoadAll()
        {
            this.masterVolume = PlayerPrefs.GetFloat("masterVolume");
            this.masterS.SetValueWithoutNotify(this.masterVolume);
            this.masterOn = this.IntToBool(PlayerPrefs.GetInt("masterOn"));
            this.masterT.SetIsOnWithoutNotify(this.masterOn);

            this.bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
            this.bgmS.SetValueWithoutNotify(this.bgmVolume);
            this.bgmOn = this.IntToBool(PlayerPrefs.GetInt("bgmOn"));
            this.bgmT.SetIsOnWithoutNotify(this.bgmOn);

            this.sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            this.sfxS.SetValueWithoutNotify(this.sfxVolume);
            this.sfxOn = this.IntToBool(PlayerPrefs.GetInt("sfxOn"));
            this.sfxT.SetIsOnWithoutNotify(this.sfxOn);
        }
    }
}
