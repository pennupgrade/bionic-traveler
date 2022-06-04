namespace BionicTraveler.Scripts.AI.MechaDog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class WatchdogBehavior : EntityBehavior
    {
        private Entity entityTarget;
        private GameTime idleStart;
        private GameTime patrolStart;
        private GameTime combatStart;

        [SerializeField]
        private Animator anim;

        [SerializeField]
        private Rigidbody2D rb;

        [SerializeField]
        [Tooltip("How long the watchdog stays in Idle state")]
        private float idleTime = 5f;
        [SerializeField]
        [Tooltip("How long the watchdog stays in Patrol state")]
        private float patrolTime = 10f;
        [SerializeField]
        [Tooltip("How long the watchdog stays in Combat state")]
        private float alertTime = 10f;

        [SerializeField]
        [Tooltip("Cooldown for the watchdog's blinding attack")]
        private float blindingCooldown = 5f;

        [SerializeField]
        [Tooltip("Range of the Watchdog's melee attack")]
        private float meleeMax = 10f;

        [SerializeField]
        [Tooltip("Range of the Watchdog's melee attack")]
        private float meleeMin = 5f;

        [SerializeField]
        private AudioClip shootSound;

        [SerializeField]
        private AudioClip changeColorSound;

        [SerializeField]
        private AudioClip dashSound;

        [SerializeField]
        private AudioClip deathSound;

        private EntityTask combatTask = null;

        private Vector3 homePos;
        private GameTime blindTime;
        private GameTime taskEnded;

        [SerializeField]
        private GameObject charge;

        public void PlayDeathSound(Entity sender, Entity killer)
        {
            AudioManager.Instance.PlayOneShot(deathSound);
            this.Owner.Dying -= this.PlayDeathSound;
        }
        private void Start()
        {
            this.Owner.Dying += this.PlayDeathSound;
            this.homePos = this.transform.position;
            this.rb = this.GetComponent<Rigidbody2D>();
            this.anim = this.GetComponent<Animator>();
        }


        public enum WatchdogStates
        {
            Idle,
            Patrol,
            Combat
        }

        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<WatchdogStates>();
            fsm.SetDefaultState(WatchdogStates.Idle);
            fsm.RegisterCallback(WatchdogStates.Idle, this.IdleState);
            fsm.RegisterCallback(WatchdogStates.Patrol, this.PatrolState);
            fsm.RegisterCallback(WatchdogStates.Combat, this.CombatState);
            return fsm;
        }

        private void IdleState(FSM<WatchdogStates> sender, WatchdogStates currentState, FSMSubState subState)
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
                    if (this.NearTargets())
                    {
                        sender.AdvanceTo(WatchdogStates.Combat);
                    }

                    if (this.idleStart.HasTimeElapsed(this.idleTime))
                    {
                        sender.AdvanceTo(WatchdogStates.Patrol);
                    }

                    break;

                case FSMSubState.Leave:
                    Debug.Log("Idle - Leave");
                    break;
            }
        }
        
        private void PatrolState(FSM<WatchdogStates> sender, WatchdogStates currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    Debug.Log("Patrol - Enter");
                    this.Owner.TaskManager.ClearTasks();

                    // TODO: Support walking/speed.
                    var homeTask = new TaskGoToPoint(this.Owner, this.homePos);
                    var patrolTask = new TaskPatrol(this.Owner, PatrolType.Square);
                    var seq = new TaskSequence(homeTask, patrolTask);
                    var exec = new TaskExecuteSequence(this.Owner, seq);
                    exec.Assign();

                    // Set patrolStart.
                    this.patrolStart = GameTime.Now;
                    break;

                case FSMSubState.Remain:
                    Debug.Log("Patrol - Remain");
                    if (this.NearTargets())
                    {
                        sender.AdvanceTo(WatchdogStates.Combat);
                    }

                    if (this.patrolStart.HasTimeElapsed(this.patrolTime))
                    {
                        sender.AdvanceTo(WatchdogStates.Idle);
                    }

                    break;

                case FSMSubState.Leave:
                    Debug.Log("Patrol - Leave");
                    break;
            }
        }

        private void CombatState(FSM<WatchdogStates> sender, WatchdogStates currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    this.taskEnded = GameTime.Now;

                    // Set combatStart.
                    this.combatStart = GameTime.Now;
                    this.combatTask = null;
                    this.anim.SetInteger("inCombat", 1);
                    break;
                case FSMSubState.Remain:
                    float dist = this.transform.DistanceTo(this.entityTarget.transform);

                    // If in range to charge, then charge
                    if (dist < this.meleeMax)
                    {
                        if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.WatchdogCharge) && this.taskEnded.HasTimeElapsed(1f))
                        {
                            this.Owner.TaskManager.ClearTasks();
                            this.combatTask = new TaskWatchdogCharge(this.Owner, this.entityTarget, this.charge);
                            this.combatTask.Ended += this.CombatTask_Ended;
                            this.combatTask.Assign();
                            AudioManager.Instance.PlayOneShot(dashSound);
                        }
                    }

                    // If in range to blind, try to blind
                    else if (dist < this.EntityScanner.DetectionRange)
                    {
                        if (this.blindTime == null || this.blindTime.HasTimeElapsed(this.blindingCooldown))
                        {
                            if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Combat) && this.taskEnded.HasTimeElapsed(1f))
                            {
                                this.Owner.TaskManager.ClearTasks();
                                this.combatTask = new TaskCombat(this.Owner, this.entityTarget);
                                this.combatTask.Ended += this.CombatTask_Ended;
                                this.combatTask.Assign();
                                this.blindTime = GameTime.Now;
                                AudioManager.Instance.PlayOneShot(shootSound);
                            }
                        }
                    }
                    else
                    {
                        if (this.combatStart.HasTimeElapsed(this.alertTime))
                        {
                            sender.AdvanceTo(WatchdogStates.Patrol);
                        }
                    }

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Leaving Combat!");
                    this.anim.SetInteger("inCombat", 0);
                    break;
            }
        }

        private void CombatTask_Ended(EntityTask task, bool successful)
        {
            this.taskEnded = GameTime.Now;
        }

        private bool NearTargets()
        {
            // If we have an entity scanner and current state is not idle, attack close targets. TODO: Configure via flags
            // such as attack targets on sight or not. Also add LOS checks.
            // TODO: Prioritize closest target.
            if (this.EntityScanner != null)
            {
                var nearbyTargets = this.EntityScanner.GetAllDynamicInRange();
                var target = nearbyTargets.FirstOrDefault(target => this.IsValidTarget(target));
                if (target != null)
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (this.enabled && collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit Player!!");
                collision.gameObject.GetComponent<PlayerEntity>().Kill();
            }
        }
    }
}
