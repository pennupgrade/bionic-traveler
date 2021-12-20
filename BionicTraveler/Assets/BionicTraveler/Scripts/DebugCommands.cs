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
            DevConsole.singleton.AddCommand(new ActionCommand<bool>(this.NoTarget));
            DevConsole.singleton.AddCommand(new ActionCommand(this.Die));
            DevConsole.singleton.AddCommand(new ActionCommand(this.DieLowEnergy));
            DevConsole.singleton.AddCommand(new ActionCommand<float>(this.SetEnergy));
            DevConsole.singleton.AddCommand(new ActionCommand<int>(this.SetHealth));
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

        private void NoTarget(bool value)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerEntity>().IsIgnoredByEveryone = value;
        }

        private void Die()
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.LoseHealth(player.Health);
        }

        private void DieLowEnergy()
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.RemoveEnergy(player.Energy);
        }

        private void SetEnergy(float value)
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.SetEnergy(value);
        }

        private void SetHealth(int value)
        {
            var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
            player.SetHealth(value);
        }
    }
}
