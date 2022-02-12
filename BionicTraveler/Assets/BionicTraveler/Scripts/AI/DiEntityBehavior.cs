namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using BionicTraveler.Assets.Framework;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class DiEntityBehavior : EntityBehavior
    {
        public enum DiEntityGoal
        {
            Idle,
            Patrol,
            Combat
        }

        private Entity entityTarget;
        private Vector3 startPosition;
        private IFSM fsm;
        private GameTime idleStart;
        private GameTime patrolStart;
        private GameTime combatStart;
        private GameTime dashStart;

        protected EntityScanner EntityScanner { get; private set; }
        protected DynamicEntity Owner { get; private set; }
        protected EntityIntelligence Intelligence { get; private set; }

        private void Awake()
        {
            this.Owner = this.GetComponent<DynamicEntity>();
            this.EntityScanner = this.GetComponent<EntityScanner>();
            this.Intelligence = this.GetComponent<EntityIntelligence>();
            this.startPosition = this.Owner.transform.position;
        }
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<DiEntityGoal>();
            fsm.SetDefaultState(DiEntityGoal.Idle);
            fsm.RegisterCallback(DiEntityGoal.Idle, this.IdleMode);
            fsm.RegisterCallback(DiEntityGoal.Patrol, this.PatrolMode);
            fsm.RegisterCallback(DiEntityGoal.Combat, this.CombatMode);
            return fsm;
        }

        /// <summary>
        /// Stay in idle mode for 5 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentState"></param>
        /// <param name="subState"></param>
        private void IdleMode(FSM<DiEntityGoal> sender, DiEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Idle - Enter");
                    this.Owner.TaskManager.ClearTasks();

                    // Remove target
                    this.entityTarget = null;

                    // set idleTime
                    this.idleStart = GameTime.Now;
                    break;
                case FSMSubState.Remain:
                    // Waits for 5 seconds before going to Patrol. Does nothing in these 5 seconds.
                    if (this.idleStart.HasTimeElapsed(5f))
                    {
                        sender.AdvanceTo(DiEntityGoal.Patrol);
                    }

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Idle - Leave");
                    break;
            }
        }

        /// <summary>
        /// 11.7 seconds and then return to patrol mode, although it may very well still be in range of the player so it will just attack again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentState"></param>
        /// <param name="subState"></param>
        private void CombatMode(FSM<DiEntityGoal> sender, DiEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    var combatTask = new TaskCombat(this.Owner, this.entityTarget);
                    combatTask.Assign();

                    // Set combatStart.
                    this.combatStart = GameTime.Now;

                    Debug.Log("Combat - Enter");
                    break;
                case FSMSubState.Remain:
                    Debug.Log("Combat - Remain");
                    if (this.combatStart.HasTimeElapsed(3f))
                    {
                        this.dashStart = GameTime.Now;
                        var dashTask = new TaskDash(this.Owner);
                        dashTask.Assign();

                        if (this.dashStart.HasTimeElapsed(3f))
                        {
                            var newCombatTask = new TaskCombat(this.Owner, this.entityTarget);
                            newCombatTask.Assign();
                        }
                    }

                    if (this.combatStart.HasTimeElapsed(11.7f))
                    {
                        sender.AdvanceTo(DiEntityGoal.Patrol);
                    }

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Combat - Leave");
                    break;
            }
        }

        /// <summary>
        /// Entity should stay in patrol mode for 10 seconds, then go back to idle mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentState"></param>
        /// <param name="subState"></param>
        private void PatrolMode(FSM<DiEntityGoal> sender, DiEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Patrol - Enter");
                    this.Owner.TaskManager.ClearTasks();

                    // TODO: Support walking/speed.
                    var patrolTask = new TaskPatrol(this.Owner, PatrolType.Square);
                    patrolTask.Assign();

                    // Set patrolStart.
                    this.patrolStart = GameTime.Now;

                    break;
                case FSMSubState.Remain:
                    Debug.Log("Patrol - Remain");

                    if (this.patrolStart.HasTimeElapsed(10f))
                    {
                        sender.AdvanceTo(DiEntityGoal.Idle);
                    }

                    // If detected valid entities to attack, then attack.
                    if (this.CheckForNearbyTargets())
                    {
                        sender.AdvanceTo(DiEntityGoal.Combat);
                    }

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Patrol - Leave");

                    break;
            }
        }

        // Helper functions.
        private bool CheckForNearbyTargets()
        {
            // If we have an entity scanner and current state is not idle, attack close targets. TODO: Configure via flags
            // such as attack targets on sight or not. Also add LOS checks.
            // TODO: Prioritize closest target.
            if (this.EntityScanner != null)
            {
                var nearbyTargets = this.EntityScanner.GetAllDynamicInRange();
                var target = nearbyTargets.FirstOrDefault();
                if (target != null && this.IsValidTarget(target))
                {
                    this.entityTarget = target;
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
