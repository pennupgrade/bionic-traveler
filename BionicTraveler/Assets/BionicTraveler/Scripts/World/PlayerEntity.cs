namespace BionicTraveler.Scripts.World
{
    using System.Linq;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.Interaction;
    using BionicTraveler.Scripts.Items;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Player Entity class.
    /// </summary>
    public class PlayerEntity : DynamicEntity
    {
        /// <summary>
        /// Gets or sets the Player interaction range.
        /// </summary>
        public float InteractionRange { get; set; } = 1;

        // TODO: Uncomment these after merging Combat classes
        private CombatBehaviour PrimaryWeapon;
        private CombatBehaviour SecondaryWeapon;
        //private List<Chip> ActiveChips = new List<Chip>();

        private int batteryHealth = 50;

        /// <summary>
        /// Deals an amount of damage to the player (for testing purposes).
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        public void DamageBattery(int damage)
        {
            this.batteryHealth = Mathf.Max(0, this.batteryHealth - damage);
            Debug.Log(this.Health);
        }

        /// <summary>
        /// Heals the player's battery health to full (for testing purposes).
        /// </summary>
        public void HealBattery()
        {
            this.batteryHealth = 50;
            Debug.Log(this.Health);
        }

        private void Update()
        {
            // TODO: This is inefficient in an open world, refine later; Create helper function to find objects of a certain type
            var interactables = GameObject.FindGameObjectsWithTag("Interactable").Where(
                interactable => Vector3.Distance(
                    interactable.transform.position, this.transform.position) < this.InteractionRange).ToArray();

            foreach (var a in interactables)
            {
                a.GetComponent<IInteractable>().OnInteract(this.gameObject);
            }

            //player pickup
            var pickups = GameObject.FindObjectsOfType<Pickup>().Where(pickup => Vector3.Distance(
                pickup.transform.position,
                this.transform.position) < this.InteractionRange).ToArray();

            foreach (var pickup in pickups)
            {
                if (pickup.transform.DistanceTo(pickup.transform) < pickup.PickUpRange)
                {
                    pickup.PickUp(this);
                }
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

            if (Input.GetKeyDown(KeyCode.O) && this.Inventory.Items.Count > 0)
            {
                var item = this.Inventory.DropItem(this.Inventory.Items.First().ItemData);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                EnemyCreator.SpawnEnemy(this.transform.position);
            }
        }

        public override void Kill()
        {
            // TODO: Move to something more general, like maybe PlayerRespawnManager?
            this.SetHealth(0);
            LevelLoadingManager.Instance.FinishedLoading += this.Instance_FinishedLoading;
            LevelLoadingManager.Instance.ReloadCurrentScene();
        }

        private void Instance_FinishedLoading()
        {
            LevelLoadingManager.Instance.FinishedLoading -= this.Instance_FinishedLoading;
            this.SetHealth(100);
        }

        private void ActivateAbility(Bodypart b)
        {
            b.ActivateAbility();
        }
    }
}