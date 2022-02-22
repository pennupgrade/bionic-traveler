namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    public enum FSMSubState
    {
        Enter,
        Remain,
        Leave,
    }

    public interface IFSM
    {
        void Process();
    }

    public class FSM<T> : IFSM
        where T : System.Enum
    {
        public delegate void FSMEventHandler(FSM<T> sender, T currentState, FSMSubState subState);

        public event FSMEventHandler Tick;

        public T CurrentState;
        public FSMSubState SubState;

        private bool isTransitioning;
        private T nextState;
        private T defaultState;
        private bool hasInitialized;

        private Dictionary<T, FSMEventHandler> callbacks;

        public FSM()
        {
            this.callbacks = new Dictionary<T, FSMEventHandler>();
        }

        public void SetDefaultState(T state)
        {
            this.defaultState = state;
        }

        public void Process()
        {
            if (!this.hasInitialized)
            {
                this.CurrentState = this.defaultState;
                this.SubState = FSMSubState.Enter;
                this.hasInitialized = true;
            }

            this.Tick?.Invoke(this, this.CurrentState, this.SubState);
            if (this.callbacks.TryGetValue(this.CurrentState, out FSMEventHandler callback))
            {
                callback(this, this.CurrentState, this.SubState);
            }

            if (this.isTransitioning)
            {
                this.SubState = FSMSubState.Enter;
                this.CurrentState = this.nextState;
                this.isTransitioning = false;
            }
            else
            {
                if (this.SubState == FSMSubState.Enter)
                {
                    this.SubState = FSMSubState.Remain;
                }
                else if (this.SubState == FSMSubState.Leave)
                {
                    this.isTransitioning = true;
                }
            }
        }

        public void RegisterCallback(T state, FSMEventHandler callback)
        {
            this.callbacks.Add(state, callback);
        }

        public void AdvanceTo(T nextState)
        {
            if (this.SubState != FSMSubState.Remain)
            {
                throw new InvalidOperationException("Already transitioning state");
            }

            this.nextState = nextState;
            this.SubState = FSMSubState.Leave;
        }
    }

    public abstract class EntityBehavior : MonoBehaviour
    {
        [NonSerialized]
        private Vector3 startPosition;
        [NonSerialized]
        private IFSM fsm;

        [field: NonSerialized]
        protected EntityScanner EntityScanner { get; private set; }
        [field: NonSerialized]
        protected DynamicEntity Owner { get; private set; }
        [field: NonSerialized]
        protected EntityIntelligence Intelligence { get; private set; }
        protected Vector3 StartPosition => this.startPosition;

        public virtual void Awake()
        {
            this.Owner = this.GetComponent<DynamicEntity>();
            this.EntityScanner = this.GetComponent<EntityScanner>();
            this.Intelligence = this.GetComponent<EntityIntelligence>();
            this.startPosition = this.Owner.transform.position;
        }

        public abstract IFSM CreateFSM();

        private void FixedUpdate()
        {
            if (this.fsm == null)
            {
                this.fsm = this.CreateFSM();
            }

            this.fsm.Process();
        }
    }

    public class DefaultEntityBehavior : EntityBehavior
    {
        [SerializeField]
        [Tooltip("The entity flags.")]
        private EntityFlags flags;

        private Entity combatTarget;

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
                    var combatTask = new TaskCombat(this.Owner, this.combatTarget);
                    combatTask.Assign();

                    Debug.Log("Combat - Enter");
                    break;
                case FSMSubState.Remain:
                    Debug.Log("Combat - Remain");
                    break;
                case FSMSubState.Leave:
                    Debug.Log("Combat - Leave");
                    break;
            }
        }
    }
}
