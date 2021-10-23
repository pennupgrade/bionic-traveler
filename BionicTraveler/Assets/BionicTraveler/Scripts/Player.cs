namespace BionicTraveler.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Temporary player movement/health/stats for testing interaction.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5.0f;
        [SerializeField]
        private Text textHealth;

        private Rigidbody2D rb;

        private int batteryHealth = 50;

        /// <summary>
        /// Deals an amount of damage to the player (for testing purposes).
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        public void DamageBattery(int damage)
        {
            this.batteryHealth -= damage;
            this.UpdateTextHealth();
        }

        /// <summary>
        /// Heals the player's battery health to full (for testing purposes).
        /// </summary>
        public void HealBattery()
        {
            this.batteryHealth = 50;
            this.UpdateTextHealth();
        }

        private void Start()
        {
            this.rb = this.GetComponent<Rigidbody2D>();

            this.UpdateTextHealth();
        }

        private void FixedUpdate()
        {
            Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            this.rb.MovePosition(this.rb.position + (movement.normalized * (Time.deltaTime * this.moveSpeed)));
        }

        private void UpdateTextHealth()
        {
            this.textHealth.text = "Battery: " + this.batteryHealth;
        }
    }
}
