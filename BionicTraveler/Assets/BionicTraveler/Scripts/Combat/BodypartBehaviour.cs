namespace BionicTraveler.Scripts.Combat
{
    using System;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.Items;
    using UnityEngine;

    /// <summary>
    /// The bodypart priority, i.e. primary or secondary.
    /// </summary>
    public enum BodypartPriority
    {
        /// <summary>
        /// The first empty priority slot.
        /// </summary>
        Any = -1,

        /// <summary>
        /// The primary bodypart.
        /// </summary>
        Primary = 0,

        /// <summary>
        /// The secondary bodypart.
        /// </summary>
        Secondary = 1,
    }

    /// <summary>
    /// Manages the behaviour of bodyparts.
    /// </summary>
    public class BodypartBehaviour : MonoBehaviour
    {
        private Dictionary<BodypartSlot, Bodypart> bodyparts;
        private Bodypart[] sortedBodyparts;
        private Bodypart currentBodypart;

        // Start is called just before any of the Update methods is called the first time
        private void Start()
        {
            Debug.Log("Hello World");
            this.bodyparts = new Dictionary<BodypartSlot, Bodypart>();
            this.sortedBodyparts = new Bodypart[Enum.GetValues(typeof(BodypartPriority)).Length - 1];
        }

        /// <summary>
        /// Equips <paramref name="bodypart"/>.
        /// </summary>
        /// <param name="bodypart">The bodypart.</param>
        /// <returns>Whether or not the bodypart was equipped.</returns>
        public bool EquipBodypart(Bodypart bodypart)
        {
            return this.EquipBodypart(bodypart, BodypartPriority.Any);
        }

        /// <summary>
        /// Equips <paramref name="bodypart"/>.
        /// </summary>
        /// <param name="bodypart">The bodypart.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>Whether or not the bodypart was equipped.</returns>
        public bool EquipBodypart(Bodypart bodypart, BodypartPriority priority)
        {
            if (bodypart is null)
            {
                throw new ArgumentNullException(nameof(bodypart));
            }

            if (!this.IsSlotEmpty(bodypart.Slot))
            {
                return false;
            }

            var targetPriority = this.GetEmptyBodypartPriority(priority);
            if (!targetPriority.FoundPriority)
            {
                throw new ArgumentException($"{nameof(priority)} is not available.");
            }

            this.bodyparts.Add(bodypart.Slot, bodypart);
            this.sortedBodyparts[(int)targetPriority.Priority] = bodypart;

            return true;
        }

        private (bool FoundPriority, BodypartPriority Priority) GetEmptyBodypartPriority(BodypartPriority priority)
        {
            if (priority == BodypartPriority.Any)
            {
                if (this.sortedBodyparts[0] == null)
                {
                    return (true, BodypartPriority.Primary);
                }
                else if (this.sortedBodyparts[1] == null)
                {
                    return (true, BodypartPriority.Secondary);
                }

                return (false, priority);
            }

            if (this.sortedBodyparts[(int)priority] == null)
            {
                return (true, priority);
            }

            return (false, priority);
        }

        /// <summary>
        /// Unequips <paramref name="bodypart"/>.
        /// </summary>
        /// <param name="bodypart">The bodypart.</param>
        public void UnequipBodypart(Bodypart bodypart)
        {
            if (bodypart is null)
            {
                throw new ArgumentNullException(nameof(bodypart));
            }

            if (this.bodyparts[bodypart.Slot] != bodypart)
            {
                throw new InvalidOperationException($"{nameof(bodypart)} is not equipped in {bodypart.Slot}.");
            }

            this.UnequipBodypart(bodypart.Slot);
        }

        /// <summary>
        /// Unequips <paramref name="slot"/>.
        /// </summary>
        /// <param name="slot">The bodypart.</param>
        public void UnequipBodypart(BodypartSlot slot)
        {
            if (this.IsSlotEmpty(slot))
            {
                throw new InvalidOperationException($"{nameof(slot)} is empty.");
            }

            this.bodyparts.Remove(slot);
        }

        /// <summary>
        /// Returns a value indicating whether <paramref name="slot"/> is empty or not.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>A value indicating whether the slot is empty.</returns>
        public bool IsSlotEmpty(BodypartSlot slot) => !this.bodyparts.ContainsKey(slot);

        /// <summary>
        /// Activates a bodyparts avility in <paramref name="priority"/>.
        /// </summary>
        /// <param name="priority">The slot.</param>
        public void ActivateAbility(BodypartPriority priority)
        {
            if (priority == BodypartPriority.Any)
            {
                throw new ArgumentException($"{nameof(BodypartPriority.Any)} is not a valid priority.");
            }

            var bodypart = this.sortedBodyparts[(int)priority];
            bodypart?.ActivateAbility();
            this.currentBodypart = bodypart;
        }

        // Update is called every frame, if the MonoBehaviour is enabled
        private void Update()
        {
            if (Input.GetButtonDown("ActivatePrimaryBodypartAbility"))
            {
                this.ActivateAbility(BodypartPriority.Primary);
            }
        }

        /// <summary>
        /// Called by Animation Events that live on the player Animator and uses the
        /// reflection to redirect the call a method on the active body part with the 
        /// name that has been passed in as eventName (from the Unity Inspector).
        /// </summary>
        /// <param name="eventName">The name of the function to call in the body part.</param>
        public void InvokeAnimationEvent(string eventName)
        {
            var bodypartType = this.currentBodypart.GetType();
            var method = bodypartType.GetMethod(eventName);
            method.Invoke(this.currentBodypart, new object[0]);
        }
    }
}