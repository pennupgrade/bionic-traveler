namespace BionicTraveler.Scripts.World
{
    using UnityEngine;
    using UnityEngine.UI;

    public class BattteryBar : MonoBehaviour
    {
        public Slider slider;
        public PlayerEntity player;

        public void Start()
        {
            this.player = GameObject.FindWithTag("Player").GetComponent<PlayerEntity>();
        }

        public void Update()
        {
            this.slider.minValue = 0;
            this.slider.maxValue = 100;
            this.slider.value = this.player.batteryHealth;
        }
    }
}