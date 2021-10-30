using UnityEngine.Audio;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>

        public AudioMixer bgmMixer, sfxMixer, voiceMixer;
        
        //public float bgmMixerVal, sfxMixerVal, voiceMixerVal; //debug
        
        private float masterVolume;
        private bool masterOn;

        private float bgmVolume;
        private bool bgmOn;

        private float sfxVolume;
        private bool sfxOn;
        
        private float voiceVolume;
        private bool voiceOn;

        public int difficulty;

        public void SetDifficulty(int diff)
        {
            difficulty = diff;
        }
        
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

        public void SetMaster(float masterSlider)
        {
            masterVolume = masterSlider;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
            }

        public void MuteMaster(bool muteMaster)
        {
            masterOn = muteMaster;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }
        
        public void SetBgm(float bgmSlider)
        {
            bgmVolume = bgmSlider;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
        }

        public void MuteBgm(bool muteBgm)
        {
            bgmOn = muteBgm;
            LogScaling(bgmMixer, masterVolume, masterOn, "bgmVolume", bgmVolume, bgmOn);
        }
        
        public void SetSfx(float sfxSlider)
        {
            sfxVolume = sfxSlider;
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
        }
        
        public void MuteSfx(bool muteSfx)
        {
            sfxOn = muteSfx;
            LogScaling(sfxMixer, masterVolume, masterOn, "sfxVolume", sfxVolume, sfxOn);
        }

        public void SetVoice(float voiceSlider)
        {
            voiceVolume = voiceSlider;
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }

        public void MuteVoice(bool muteVoice)
        {
            voiceOn = muteVoice;
            LogScaling(voiceMixer, masterVolume, masterOn, "voiceVolume", voiceVolume, voiceOn);
        }
    }
}
