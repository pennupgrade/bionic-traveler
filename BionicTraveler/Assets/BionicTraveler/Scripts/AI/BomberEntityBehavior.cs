namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    public class BomberEntityBehavior : EntityBehavior
    {
        [SerializeField]
        [Tooltip("The entity flags.")]
        private EntityFlags flags;

        private Entity combatTarget;
        private TaskAttack explosionAttack;
        private TaskMoveToEntity followTask;

        /// <summary>
        /// Describes an entity's main goal.
        /// </summary>
        public enum EntityGoal
        {
            /// <summary>
            /// The entity is idle.
            /// </summary>
            Idle,

            /// <summary>
            /// The entity patrols.
            /// </summary>
            Patrol,

            /// <summary>
            /// The entity fights.
            /// </summary>
            Combat,
        }

        /// <summary>
        /// Provides options to configure an entity's behavior.
        /// </summary>
        [System.Serializable]
        [System.Flags]
        public enum EntityFlags
        {
            /// <summary>
            /// Whether an entity will attack an enemy on sight.
            /// </summary>
            AttackOnSight = 0x1,

            /// <summary>
            /// Whether an entity will not attack an enemy when idling.
            /// </summary>
            DontAttackWhenIdle = 0x2,
        }

        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<EntityGoal>();
            fsm.SetDefaultState(EntityGoal.Idle);
            fsm.RegisterCallback(EntityGoal.Idle, this.IdleState);
            fsm.RegisterCallback(EntityGoal.Patrol, this.PatrolState);
            fsm.RegisterCallback(EntityGoal.Combat, this.CombatState);
            return fsm;
        }

        private void IdleState(FSM<EntityGoal> sender, EntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Idle - Enter");
                    this.Owner.TaskManager.ClearTasks();
                    break;
                case FSMSubState.Remain:
                    if (!this.flags.HasFlag(EntityFlags.DontAttackWhenIdle))
                    {
                        if (this.CheckForNearbyTargets())
                        {
                            sender.AdvanceTo(EntityGoal.Combat);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.U))
                    {
                        this.combatTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
                        sender.AdvanceTo(EntityGoal.Combat);
                    }

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Idle - Leave");
                    break;
            }
        }

        private void PatrolState(FSM<EntityGoal> sender, EntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Patrol - Enter");
                    this.Owner.TaskManager.ClearTasks();

                    // TODO: Support walking/speed.
                    var patrolTask = new TaskPatrol(this.Owner, PatrolType.Square);
                    patrolTask.Assign();
                    break;
                case FSMSubState.Remain:
                    Debug.Log("Patrol - Remain");
                    break;
                case FSMSubState.Leave:
                    Debug.Log("Patrol - Leave");
                    break;
            }
        }

        private void CombatState(FSM<EntityGoal> sender, EntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    this.followTask = new TaskMoveToEntity(this.Owner, this.combatTarget);
                    followTask.Assign();
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
                    }
                    else
                    {
                        if (!followTask.IsActive)
                        {
                            this.followTask = new TaskMoveToEntity(this.Owner, this.combatTarget);
                            followTask.Assign();
                        } else
                        {
                            
                            Vector3 ownerPos = this.Owner.transform.position;
                            Vector3 tgtPos = this.combatTarget.transform.position;
                            var distanceToTarget = Vector3.Magnitude(tgtPos - ownerPos);

                            if (distanceToTarget > 10f)
                            {
                                followTask.End("Out of Range", true);
                                sender.AdvanceTo(EntityGoal.Idle);
                            }
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
                    }
                    break;
                case FSMSubState.Leave:
                    Debug.Log("Combat - Leave");
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
                var target = nearbyTargets.FirstOrDefault();
                if (target != null && this.IsValidTarget(target))
                {
                    if (this.flags.HasFlag(EntityFlags.AttackOnSight))
                    {
                        this.combatTarget = target;
                        return true;
                    }
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
