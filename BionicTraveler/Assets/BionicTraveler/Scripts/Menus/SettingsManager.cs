namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.Audio;
    using UnityEngine;

    /// <summary>
    /// Script should be attached to the GameObject 'SettingsMenu'
    /// GameObject 'SettingsMenu' should be dragged in as the SerializedField 'canvas'
    /// </summary>
    public class SettingsManager : Menu
    {
        [SerializeField]
        private AudioClip clickSound;
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
            AudioManager.Instance.PlayOneShot(clickSound);
            this.difficulty = diff;
        }

        private void SetMaster(float masterSlider)
        {
            this.masterVolume = masterSlider;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetFloat("masterVolume", this.masterVolume);
        }

        private void MuteMaster(bool muteMaster)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.masterOn = muteMaster;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetInt("masterOn", this.BoolToInt(this.masterOn));
        }

        private void SetBgm(float bgmSlider)
        {
            this.bgmVolume = bgmSlider;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetFloat("bgmVolume", this.bgmVolume);
        }

        private void MuteBgm(bool muteBgm)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.bgmOn = muteBgm;
            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetInt("bgmOn", this.BoolToInt(this.bgmOn));
        }

        private void SetSfx(float sfxSlider)
        {
            this.sfxVolume = sfxSlider;
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetFloat("sfxVolume", this.sfxVolume);
        }

        private void MuteSfx(bool muteSfx)
        {
            AudioManager.Instance.PlayOneShot(clickSound);
            this.sfxOn = muteSfx;
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
            PlayerPrefs.SetInt("sfxOn", this.BoolToInt(this.sfxOn));
        }

        private void SetVoice(float voiceSlider)
        {
            this.voiceVolume = voiceSlider;
            AudioManager.Instance.VoiceVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.voiceVolume, this.voiceOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void MuteVoice(bool muteVoice)
        {
            this.voiceOn = muteVoice;
            AudioManager.Instance.VoiceVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.voiceVolume, this.voiceOn);
            AudioManager.Instance.UpdateVolume();
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game.
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
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

            this.LoadAll();

            AudioManager.Instance.MusicVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.bgmVolume, this.bgmOn);
            AudioManager.Instance.EffectsVolume = this.ChangeVolume(this.masterVolume, this.masterOn, this.sfxVolume, this.sfxOn);
            AudioManager.Instance.UpdateVolume();
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

        private int BoolToInt(bool b) => b ? 1 : 0;

        private bool IntToBool(int i) => i != 0;

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
