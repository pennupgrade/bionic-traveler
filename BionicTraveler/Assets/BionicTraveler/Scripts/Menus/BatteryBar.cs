namespace BionicTraveler.Scripts.Menus
{
    using System;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The visual representation of the player's battery level.
    /// </summary>
    public class BatteryBar : MonoBehaviour
    {
        private Image image;
        private PlayerEntity playerEntity;

        [SerializeField]
        private Sprite[] batteryLevels;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.image = this.GetComponent<Image>();
            var player = GameObject.FindGameObjectWithTag("Player");
            this.playerEntity = player.GetComponent<PlayerEntity>();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            // TODO: Respect max energy. More granular energy levels, perhaps drive bar from code
            // instead of using static images.
            var energyLevel = this.playerEntity.Energy;
            var imageIndex = Math.Min(10, Math.Max(0, energyLevel / 10));
            this.image.sprite = this.batteryLevels[(int)imageIndex];
        }
    }
}
