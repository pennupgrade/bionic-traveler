namespace BionicTraveler.Scripts.World
{
    using System.Linq;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Items;
    using global::Items;
    using UnityEngine;

    /// <summary>
    /// Player Entity class.
    /// </summary>
    public class PlayerEntity : DynamicEntity
    {
        private float interactionRange = 100;

        /// <summary>
        /// Gets or sets the BodyPart to activate with PrimaryBP input.
        /// </summary>
        public Bodypart PrimaryBP { get => this.primaryBP; set => this.primaryBP = value; }

        /// <summary>
        /// Gets or sets the BodyPart to activate with SecondaryBP input.
        /// </summary>
        public Bodypart SecondaryBP { get; set; }

        /// <summary>
        /// Gets or sets the Player interaction range.
        /// </summary>
        public float InteractionRange { get => interactionRange; set => interactionRange = value; }

        // TODO: Uncomment these after merging Combat classes
        private CombatBehavior PrimaryWeapon;
        //private Weapon SecondaryWeapon;
        //private List<Chip> ActiveChips = new List<Chip>();

        private Bodypart primaryBP;
        private int batteryHealth = 50;

        /// <summary>
        /// Deals an amount of damage to the player (for testing purposes).
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        public void DamageBattery(int damage)
        {
            this.batteryHealth = Mathf.Max(0, this.batteryHealth - damage);
            Debug.Log(this.GetHealth());
        }

        /// <summary>
        /// Heals the player's battery health to full (for testing purposes).
        /// </summary>
        public void HealBattery()
        {
            this.batteryHealth = 50;
            Debug.Log(this.GetHealth());
        }

        // Fixed Update is called at fixed intervals in real time
        private void FixedUpdate()
        {
            // TODO: This is inefficient in an open world, refine later; Create helper function to find objects of a certain type
            var interactables = GameObject.FindGameObjectsWithTag("Interactable").Where(
                interactable => Vector3.Distance(
                    interactable.transform.position, this.transform.position) < this.interactionRange).ToArray();

            foreach (var a in interactables)
            {
                //a.GetComponent<IInteractable>().OnInteract(this.gameObject);
            }

            //player pickup
            var pickups = GameObject.FindObjectsOfType<Pickup>().Where(pickup => Vector3.Distance(
                pickup.transform.position,
                this.transform.position) < this.interactionRange).ToArray();

            foreach (var a in pickups)
            {
                a.PickUp(this);
            }

            //press key to show inventory data
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log(this.Inventory.ToString());
            }

            //Drink Health Potion
            if (Input.GetKeyDown(KeyCode.H))
            {
                foreach (var item in Inventory.Items)
                {
                    this.Inventory.Use(item);
                }
            }
        }

        private void ActivateAbility(Bodypart b)
        {
            b.ActivateAbility();
        }
    }
}