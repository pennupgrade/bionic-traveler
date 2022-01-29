namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum EntityTaskType 
    {
        GoToPoint,
        FollowEntity,
        PickUpItem,
        Patrol
    };

    /// <summary>
    /// EntityTask is one task unit.
    /// </summary>
    public abstract class EntityTask
    {
        public DynamicEntity Owner { get; }
        public EntityTask(DynamicEntity owner)
        {
            this.Owner = owner;
        }

        public abstract EntityTaskType Type { get; }

        public bool WasInitialized { get; private set; }
        public bool WasCancelled { get; private set; }

        public bool WasSuccessful { get; private set; }

        public bool HasEnded { get; private set; }

        public bool IsActive { get; private set; }

        public delegate void EntityTaskEndedEventHandler(EntityTask task, bool successful);
        
        public event EntityTaskEndedEventHandler Ended; // Other tasks can be notified when task is done

        public virtual void OnInitialize()
        {
            // children classes can do more specific initialize tasks
        }

        // guaranteed to only be called once and before process
        public void Initialize()
        {
            this.IsActive = true;
            this.WasInitialized = true;
            this.OnInitialize();
        }

        public abstract void OnProcess();

        public void Process()
        {
            if (this.Owner.IsDead)
            {
                this.Cancel("Owner died");
                return;
            }

            // If was not initialized, then initialize.
            if (!this.WasInitialized)
            {
                this.Initialize();
            }

            this.OnProcess();
        }

        public virtual void OnEnd()
        {
            // Implemented by children classes for generic cleanup
        }

        public void End(String reason)
        {
            Debug.Log(reason);
            this.End();

        }

        private void End()
        {
            this.IsActive = false;
            this.WasSuccessful = !this.WasCancelled; // if not cancelled, then it means it was successful
            this.HasEnded = true;

            // In case we need shared logic for ending between all types
            Debug.Log("Task Ended...");
            this.OnEnd();

            this.Ended?.Invoke(this, this.WasSuccessful);
        }

        public virtual void OnCancel()
        {
            
        }

        public void Cancel(String abortMessage)
        {
            this.WasCancelled = true;

            Debug.Log("Cancelling task " + this.Type + ": " + abortMessage);
            this.OnCancel();
            this.End();
        }

    }
}
