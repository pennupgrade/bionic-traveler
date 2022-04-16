using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Used to run subscribed functions when a function in this class will run,
    /// in this case Save and Load. We want all classes to Save to the store
    /// before before SaveManager saves the store internally, and similar for load.
    /// </summary>
    public delegate void Notify();

    /// <summary>
    /// Class that manages all saving and loading.
    /// </summary>
    public class SaveManager : Menu
    {
        /// <summary>
        /// Opening saves is not always possible (only possible in fireplaces),
        /// so class takes care of own opening/closing rather than WindowManager.
        /// </summary>
        private bool isOpened;

        private Dictionary<string, object> store = new Dictionary<string, object>();
        private string[] saves = Enumerable.Repeat(string.Empty, 3).ToArray();

        [SerializeField]
        private Text saveTime1;
        [SerializeField]
        private Text saveTime2;
        [SerializeField]
        private Text saveTime3;

        private int lastSlot = -1;

        /// <summary>
        /// Subscribable event for classes that want to save to store before SaveManager store info saves internally.
        /// </summary>
        public event Notify IsSaving;

        /// <summary>
        /// Subscribable event for classes that want to load from store after SaveManager store info loads a past save.
        /// </summary>
        public event Notify IsLoading;

        /// <summary>
        /// Gets the singleton instance of SaveManager.
        /// </summary>
        public static SaveManager Instance { get; private set; }

        /// <summary>
        /// Gets if Save Menu is opened.
        /// </summary>
        /// <returns>If Save Menu is opened.</returns>
        public bool GetIsOpened()
        {
            return this.isOpened;
        }

        /// <summary>
        /// Called by other classes to save data to store.
        /// KEY NAME MUST BE COMMUNICATED WITHIN PROGRAMMING TEAM!
        /// </summary>
        /// <param name="key">String to name the value being saved.</param>
        /// <param name="value">Value being saved.</param>
        public void Save(string key, object value)
        {
            if (key == null)
            {
                Debug.Log("SaveManager.Save: key cannot be null");
            }
            else
            {
                this.store[key] = value;
            }
        }

        /// <summary>
        /// Called by other classes to load data from store.
        /// </summary>
        /// <param name="key">Name of object you want to load.</param>
        /// <returns>Returns the value of key, if found, or null.</returns>
        public object Load(string key)
        {
            this.store.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Called by SaveButton on Save Menu to save all game data.
        /// </summary>
        /// <param name="slot">Slot to save data to.</param>
        public void SaveGame(int slot)
        {
            this.IsSaving?.Invoke();
            this.SetSaveTime(slot);

            Scene scene = SceneManager.GetActiveScene();
            Save("ActiveScene", scene.name);

            Debug.Log(scene.name);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(this.GetSavePath(slot), FileMode.Create);
            bf.Serialize(stream, this.store);
            stream.Close();

            this.lastSlot = slot;
        }

        private void OnSceneLoaded()
        {
            this.IsLoading?.Invoke();
            LevelLoadingManager.Instance.FinishedLoading -= this.OnSceneLoaded;
        }

        /// <summary>
        /// Called by LoadButton on Save Menu to load all game data.
        /// </summary>
        /// <param name="slot">Slot to load data from.</param>
        public void LoadGame(int slot)
        {
            if (File.Exists(this.GetSavePath(slot)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(this.GetSavePath(slot), FileMode.Open);
                this.store = bf.Deserialize(stream) as Dictionary<string, object>;
                stream.Close();

                string loadedSceneName = (string)Load("ActiveScene");
                Scene activeScene = SceneManager.GetActiveScene();
                LevelLoadingManager.Instance.FinishedLoading += this.OnSceneLoaded;
                if (loadedSceneName == activeScene.name)
                {
                    LevelLoadingManager.Instance.ReloadCurrentScene();
                }
                else
                {
                    LevelLoadingManager.Instance.StartLoadLevel(loadedSceneName, LevelLoadingManager.DefaultSpawnPoint);
                }

                this.Close();
            }
            else
            {
                Debug.Log("File not found");
            }
        }

        /// <summary>
        /// Attempts to load the last save. If no save has been made, reloads current scene instead.
        /// </summary>
        /// <param name="reason">The reason for the reload.</param>
        /// <returns>Whether or not a save slot was found.</returns>
        public bool TryLoadLastSave(string reason)
        {
            Debug.Log(reason);

            // Loads game from the most recent save slot
            int lastSlot = SaveManager.Instance.GetLastSlot();
            // If there is no available save slot
            if (lastSlot < 0)
            {
                LevelLoadingManager.Instance.ReloadCurrentScene();
                return false;
            }
            else
            {
                SaveManager.Instance.LoadGame(lastSlot);
                return true;
            }
        }

        /// <summary>
        /// Ope) overriden to also handle keeping track of self's state with isOpened.
        /// </summary>
        public override void Open()
        {
            base.Open();
            this.isOpened = true;
        }

        /// <summary>
        /// Close overriden to also handle keeping track of self's state with isOpened.
        /// </summary>
        public override void Close()
        {
            base.Close();
            this.isOpened = false;
        }

        /// <summary>
        /// Gets the most recent saved slot
        /// </summary>
        /// <returns>Returns the number of the most recent slot that was saved to</returns>
        public int GetLastSlot()
        {
            return this.lastSlot;
        }

        private Text FindText(int slot)
        {
            return slot switch
            {
                1 => this.saveTime1,
                2 => this.saveTime2,
                3 => this.saveTime3,
                _ => null
            };
        }

        private void SetSaveTime(int slot)
        {
            Text time = this.FindText(slot);
            time.text = DateTime.UtcNow.ToLocalTime().ToString("MM/dd/yy hh:mm tt");
            this.saves[slot - 1] = time.text;
            this.SaveSaves();
        }

        private void LoadSaveTime(int slot)
        {
            Text time = this.FindText(slot + 1);
            time.text = this.saves[slot];
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
                this.LoadSaves();
            }
        }

        private void SaveSaves()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(this.GetSavePath(0), FileMode.Create);
            bf.Serialize(stream, this.saves);
            stream.Close();
        }

        private void LoadSaves()
        {
            if (File.Exists(this.GetSavePath(0)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(this.GetSavePath(0), FileMode.Open);
                this.saves = bf.Deserialize(stream) as string[];
                stream.Close();
            }

            for (int i = 0; i < this.saves.Length; i++)
            {
                this.LoadSaveTime(i);
            }
        }

        private string GetSavePath(int slot)
        {
            return Application.persistentDataPath + "/saveData" + slot + ".dat";
        }

        // Example Event Handler:
        // private int X = 50;
        // private SaveManager saveManager = SaveManager.Instance;
        //
        // public void Start() {
        //     this.saveManager.IsSaving += this.Save;
        //     this.saveManager.IsLoading += this.Load;
        // }
        //
        // public void Save() {
        //     this.saveManager.Save("playerX", X)
        // }
        //
        // public void Load() {
        //     this.X = this.saveManager.Load("playerX")
        // }
    }
}