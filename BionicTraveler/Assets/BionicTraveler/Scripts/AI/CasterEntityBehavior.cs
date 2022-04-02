namespace BionicTraveler.Scripts.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    public class CasterEntityBehavior : EntityBehavior
    {
        [SerializeField]
        [Tooltip("The entity flags.")]
        private EntityFlags flags;
        private GameTime castTime;

        [SerializeField]
        private GameObject aoe;

        private Entity combatTarget;
        private List<GameObject> activeAttacks;

        [SerializeField]
        private AudioClip teleportInSound;

        [SerializeField]
        private AudioClip teleportOutSound;

        [SerializeField]
        private AudioClip casterAttackSound;

        [SerializeField]
        private AudioClip deathSound;

        /// <summary>
        /// Initializes a new instance of the <see cref="CasterEntityBehavior"/> class.
        /// </summary>
        public CasterEntityBehavior()
        {
            this.activeAttacks = new List<GameObject>();
        }

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

        public void PlayDeathSound(Entity sender, Entity killer)
        {
            AudioManager.Instance.PlayOneShot(deathSound);
            this.Owner.Dying -= this.PlayDeathSound;
        }

        public override void Awake()
        {
            base.Awake();
            this.Owner.Dying += this.PlayDeathSound;
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
            Debug.Log("didn't ignore");
            if (!this.Owner.Relationships.IsHostile(target.tag))
            {
                return false;
            }
            Debug.Log("is hostile");
            if (Vector3.Distance(this.transform.position, target.transform.position) > this.Intelligence.CombatRange)
            {
                return false;
            }
            Debug.Log("within range");
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

                    AudioManager.Instance.PlayOneShot(teleportOutSound);

                    break;
                case FSMSubState.Remain:
                    if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Teleport))
                    {
                        AudioManager.Instance.PlayOneShot(teleportInSound);
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
                    this.castTime = GameTime.Now;

                    this.Owner.gameObject.GetComponent<Animator>().Play("Casting");

                    AudioManager.Instance.PlayOneShot(casterAttackSound);

                    GameObject spawnedAttack = GameObject.Instantiate(this.aoe, this.combatTarget.transform.position, Quaternion.identity);
                    spawnedAttack.GetComponent<AoeAttackIndicatorScript>().Initialize(this.Owner, this.combatTarget.transform);
                    this.activeAttacks.Add(spawnedAttack);

                    var moveTask = new TaskMoveToEntity(this.Owner, this.combatTarget);
                    moveTask.Assign();
                    break;
                case FSMSubState.Remain:
                    if (this.castTime.HasTimeElapsed(1.5f))
                    {
                        sender.AdvanceTo(EntityGoal.Patrol);
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private void OnDestroy()
        {
            foreach (var attack in this.activeAttacks)
            {
                if (!attack.IsDestroyed())
                {
                    Destroy(attack);
                }
            }
        }
    }
}
