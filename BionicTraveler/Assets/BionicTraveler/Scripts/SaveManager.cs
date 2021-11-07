using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BionicTraveler.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private Dictionary<string, object> store = new Dictionary<string, object>();

        private string savePath = Application.persistentDataPath + "/saveData";

        public static SaveManager Instance { get; private set; }

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

        public void AddToStore(string key, object value)
        {
            this.store.Add(key, value);
        }

        public object GetFromStore(string key)
        {
            object value = null;
            this.store.TryGetValue(key, out value);
            return value;
        }

        public void Save() {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(this.savePath, FileMode.Create);
            bf.Serialize(stream, this.store);
            stream.Close();
        }

        public void Load()
        {
            if (File.Exists(this.savePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(this.savePath, FileMode.Open);
                this.store = bf.Deserialize(stream) as Dictionary<string, object>;
                stream.Close();
            }
            else
            {
                Debug.Log("File not found");
            }
        }

        // /*
        //  *
        //  * Version 2 (saving variables upon update in global store)
        //  * scripts have variables to be saved
        //  * on variable change, the variable is changed in a big SaveData.cs file
        //  * ? which variable holds the source data? SaveData or variables in scripts?
        //  * Manager.save() gets variables from the SaveData.cs and stores in file
        //  * Manager.load() populates the SaveData.cs variables
        //  * ? how does each script know that Manager.load() is called to query from SaveData.cs
        //  * each script handles what variable they want saved and how to load it
        //  * Manager does not need to know which scripts have .save and .load
        //  * JSON vs binary
        //  *
        //  * dict string => object
        //  *
        //  * Will have startup script later
        //  */
    }
}
