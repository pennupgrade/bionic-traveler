namespace BionicTraveler.Scripts.AI
{
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// The fish spawned by a box behavior.
    /// </summary>
    public class FishBehavior : EntityBehavior
    {
        private Entity combatTarget;
        private GameObject boxBase;

        /// <summary>
        /// The goals for the fish.
        /// </summary>
        public enum FishEntityGoal
        {
            Combat,
            Flee
        }

        /// <summary>
        /// Sets the box that created this fish.
        /// </summary>
        /// <param name="box">The box.</param>
        public void SetBoxBase(GameObject box)
        {
            this.boxBase = box;
        }

        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<FishEntityGoal>();
            fsm.SetDefaultState(FishEntityGoal.Combat);
            fsm.RegisterCallback(FishEntityGoal.Combat, this.CombatState);
            fsm.RegisterCallback(FishEntityGoal.Flee, this.FleeMode);
            return fsm;
        }

        /// <summary>
        /// Stay in attack mode until player is detected within radius.
        /// </summary>
        private void CombatState(FSM<FishEntityGoal> sender, FishEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    break;
                case FSMSubState.Remain:
                    if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Combat) && this.CheckForNearbyTargets())
                    {
                        var combatTask = new TaskCombat(this.Owner, this.combatTarget);
                        combatTask.Assign();
                    }

                    if (this.Owner.Health < 100)
                    {
                        sender.AdvanceTo(FishEntityGoal.Flee);
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        /// <summary>
        /// Stay in hide mode until player is detected within radius.
        /// </summary>
        private void FleeMode(FSM<FishEntityGoal> sender, FishEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    break;
                case FSMSubState.Remain:
                    if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.GoToPoint))
                    {
                        var distance = this.Owner.transform.DistanceTo(this.boxBase.transform);
                        if (distance < 2f)
                        {
                            this.boxBase.GetComponent<HideChaseEnemyBehavior>().SetFishReturned();
                            Destroy(this.gameObject);
                        }
                        else
                        {
                            var goBackTask = new TaskGoToPoint(this.Owner, this.boxBase.transform.position);
                            goBackTask.Assign();
                        }
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private bool CheckForNearbyTargets()
        {
            // If we have an entity scanner, attack close targets. TODO: Configure via flags
            // such as attack targets on sight or not. Also add LOS checks.
            // TODO: Prioritize closest target.
            if (this.EntityScanner != null)
            {
                var nearbyTargets = this.EntityScanner.GetAllDynamicInRange();
                var target = nearbyTargets.FirstOrDefault(target => this.IsValidTarget(target));
                if (target != null && this.IsValidTarget(target))
                {
                    this.combatTarget = target;
                    return true;
                }
            }

            return false;
        }

        private bool IsValidTarget(Entity target)
        {
            var ignoreTarget = target.IsPlayer && ((PlayerEntity)target).IsIgnoredByEveryone;
            if (ignoreTarget)
            {
                return false;
            }

            if (!this.Owner.Relationships.IsHostile(target.tag))
            {
                return false;
            }

            if (Vector3.Distance(this.transform.position, target.transform.position) > this.Intelligence.CombatRange)
            {
                return false;
            }

            return true;
        }
    }
}
