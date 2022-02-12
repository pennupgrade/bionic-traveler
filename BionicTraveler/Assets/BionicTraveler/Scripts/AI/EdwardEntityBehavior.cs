namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;
    using BionicTraveler.Assets.Framework;
    using Framework;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class EdwardEntityBehavior : EntityBehavior
    {
        public enum EdwardEntityGoal
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

        private TaskAttack explosionAttack;

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
            var fsm = new FSM<EdwardEntityGoal>();
            fsm.SetDefaultState(EdwardEntityGoal.Idle);
            fsm.RegisterCallback(EdwardEntityGoal.Idle, this.IdleMode);
            //fsm.RegisterCallback(EdwardEntityGoal.Patrol, this.PatrolMode);
            fsm.RegisterCallback(EdwardEntityGoal.Combat, this.CombatMode);
            return fsm;
        }

        /// <summary>
        /// Stay in idle mode for 5 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentState"></param>
        /// <param name="subState"></param>
        private void IdleMode(FSM<EdwardEntityGoal> sender, EdwardEntityGoal currentState, FSMSubState subState)
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
                    if (this.idleStart.HasTimeElapsed(1f))
                    {
                        if (CheckForNearbyTargets())
                        {
                            sender.AdvanceTo(EdwardEntityGoal.Combat);
                        }
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
        private void CombatMode(FSM<EdwardEntityGoal> sender, EdwardEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.explosionAttack = null;
                    var followTask = new TaskFollowEntity(this.Owner, this.entityTarget);
                    followTask.Assign();

                    this.combatStart = GameTime.Now;

                    Debug.Log("Combat - Enter");
                    break;
                case FSMSubState.Remain:
                    Debug.Log("Combat - Remain");
                    if (this.explosionAttack != null)
                    {
                        if (this.explosionAttack.HasEnded)
                        {
                            if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Die))
                            {

                                var dieTask = new TaskDie(this.Owner);
                                dieTask.Assign();
                            }
                        }
                    } else
                    {
                        var distanceToTarget = this.Owner.transform.DistanceTo(this.entityTarget.transform);
                        if (distanceToTarget < 2f)
                        {
                            if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Attack))
                            {
                                var attackTask = new TaskAttack(this.Owner, true);
                                attackTask.Assign();
                                this.explosionAttack = attackTask;
                            }
                        }
                    }
                    

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Combat - Leave");
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
