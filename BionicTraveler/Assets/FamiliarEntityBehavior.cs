using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BionicTraveler.Assets.Framework;
using BionicTraveler.Scripts.AI;
using BionicTraveler.Scripts.Combat;
using BionicTraveler.Scripts.World;
using Framework;
using UnityEngine;

namespace BionicTraveler
{
    public class FamiliarEntityBehavior : EntityBehavior
    {
        [SerializeField]
        [Tooltip("The entity flags.")]
        private EntityFlags flags;
        private GameTime castTime;


        private Entity combatTarget;

        [SerializeField]
        private GameObject bomberPrefab;

        [SerializeField]
        private float spawnInterval;


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
                foreach (DynamicEntity target in nearbyTargets)
                {
                    if (target != null && this.IsValidTarget(target))
                    {
                        if (this.flags.HasFlag(EntityFlags.AttackOnSight))
                        {
                            this.combatTarget = target;
                            return true;
                        }
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

        private void PatrolState(FSM<EntityGoal> sender, EntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();

                    var teleportTask = new TaskTeleportAway(this.Owner);
                    teleportTask.Assign();

                    break;
                case FSMSubState.Remain:
                    if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Teleport))
                    {
                        sender.AdvanceTo(EntityGoal.Idle);
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private void CombatState(FSM<EntityGoal> sender, EntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    

                    this.Owner.gameObject.GetComponent<Animator>().Play("Casting"); // TODO: Replace with familiar anims once they exist

                    if (castTime == null || castTime.HasTimeElapsed(1f))
                    {
                        var spawnTask = new TaskSpawnGameObject(this.Owner, new List<GameObject> { bomberPrefab }, this.Pick);
                        spawnTask.Assign();
                        this.castTime = GameTime.Now;
                    }

                    var moveTask = new TaskMoveToEntity(this.Owner, this.combatTarget);
                    moveTask.Assign();
                    break;
                case FSMSubState.Remain:
                    if (this.castTime.HasTimeElapsed(spawnInterval))
                    {
                        sender.AdvanceTo(EntityGoal.Patrol);
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private GameObject Pick(List<GameObject> list)
        {
            return list.ToArray()[0];
        }

    }
}
