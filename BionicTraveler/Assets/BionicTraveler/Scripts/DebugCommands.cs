namespace BionicTraveler.Scripts
{
    using System;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using SickDev.CommandSystem;
    using SickDev.DevConsole;
    using UnityEngine;

    /// <summary>
    /// This class contains debug commands for the dev console.
    /// </summary>
    public class DebugCommands : MonoBehaviour
    {
        private void OnEnable()
        {
            DevConsole.singleton.AddCommand(new ActionCommand(this.PrintKeys));
        }

        private void PrintKeys()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            var keys = player.GetComponent<PlayerEntity>().KeyManager.GetKeysForCurrentLevel();
            if (keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    var sceneNames = string.Empty;
                    key.Scenes.ToList().ForEach(scene => sceneNames += scene + Environment.NewLine);
                    Debug.Log($"{key.Color} works in {sceneNames.TrimEnd()}");
                }
            }
            else
            {
                Debug.Log("You have no keys");
            }
        }
    }
}
