namespace BionicTraveler.Scripts.Items
{
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public abstract class Bodypart
    {
        [SerializeField]
        private PlayerEntity player;

        [SerializeField]
        protected Slot slotBP;
        private ItemData ItemBP;
        internal Mechanic MechanicBP;

        public PlayerEntity Player => this.player;

        public Slot SlotBP => this.slotBP;

        private void SetPrimary()
        {
            this.player.PrimaryBP = this;
        }

        private void SetSecondary()
        {
            this.player.SecondaryBP = this;
        }

        public abstract void ActivateAbility();


        public Mechanic GetMechanicType()
        {
            return MechanicBP;
        }

        public enum Mechanic
        {
            TraversalMechanic,
            AttackMechanic
        }


        public Slot GetSlot()
        {
            return this.slotBP;
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }

    public enum Slot
    {
        RightArm,
        LeftArm,
        Legs
    }
}
