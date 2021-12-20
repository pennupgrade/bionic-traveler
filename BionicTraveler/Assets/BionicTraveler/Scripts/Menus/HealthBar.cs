namespace BionicTraveler.Scripts.Menus
{
    using System;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The visual representation of the player's health.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        private PlayerEntity playerEntity;

        [SerializeField]
        private Sprite[] healthLevels;

        [SerializeField]
        private Image[] hearts;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            this.playerEntity = player.GetComponent<PlayerEntity>();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            // TODO: Respect max health. More granular health levels, maybe dynamic instead
            // of using UI for ordering?
            var health = this.playerEntity.Health;
            var healthPerHeart = 100 / this.hearts.Length;
            var fullHearts = health / healthPerHeart;
            var fullHeartSprite = this.healthLevels.Last();

            // Enable everything by default.
            foreach (var image in this.hearts)
            {
                image.enabled = true;
                image.sprite = fullHeartSprite;
            }

            if (fullHearts < this.hearts.Length)
            {
                var partialHeartIndex = fullHearts;
                var partialHeartHealth = health % healthPerHeart;
                var partialHeartSprite = Math.Min(this.healthLevels.Length - 1, partialHeartHealth / (healthPerHeart / this.healthLevels.Length));
                this.hearts[partialHeartIndex].sprite = this.healthLevels[partialHeartSprite];

                // Disable empty hearts.
                for (int i = partialHeartIndex + 1; i < this.hearts.Length; i++)
                {
                    this.hearts[i].enabled = false;
                }
            }
        }
    }
}
