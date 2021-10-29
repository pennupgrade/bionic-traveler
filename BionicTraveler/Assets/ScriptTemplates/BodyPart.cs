namespace Items
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public abstract class BodyPart
    {
        [SerializeField]
        internal PlayerEntity Player;
        internal Slot SlotBP;
        private Item ItemBP;
        internal Mechanic MechanicBP;


        private void SetPrimary()
        {
            Player.SetPrimary(this);
        }

        private void SetSecondary()
        {
            Player.SetSecondary(this);
        }

        public abstract void ActivateAbility();


        public Mechanic GetMechanic()
        {
            return MechanicBP;
        }

        public enum Mechanic
        {
            TraversalMechanic,
            AttackMechanic
        }


        public Slot getSlot()
        {
            return SlotBP;
        }

        public enum Slot
        {
            RightArm,
            LeftArm,
            Legs
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
}
