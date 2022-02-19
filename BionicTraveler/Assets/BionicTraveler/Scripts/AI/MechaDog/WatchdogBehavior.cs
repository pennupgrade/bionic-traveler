namespace BionicTraveler.Scripts.AI.MechaDog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
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

        private EntityTask combatTask = null;


        private Vector3 homePos;
        private GameTime blindTime;
        private GameTime taskEnded;

        [SerializeField]
        private GameObject charge;



        public void Start()
        {
            homePos = this.transform.position;
            rb = this.GetComponent<Rigidbody2D>();
            anim = this.GetComponent<Animator>();
        }


        public enum WatchdogStates
        {
            Idle,
            Patrol,
            Combat
        }

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

                    if (this.idleStart.HasTimeElapsed(idleTime))
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
                    this.rb.MovePosition(homePos);


                    var homeTask = new TaskGoToPoint(this.Owner, homePos);

                    Debug.Log("Patrol - Enter");
                    this.Owner.TaskManager.ClearTasks();

                    // TODO: Support walking/speed.
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

                    var dir = this.GetComponent<NavMeshAgent>().velocity.normalized;

                    Debug.Log(dir);
                    anim.SetFloat("Horizontal", dir.x);

                    anim.SetFloat("Vertical", dir.y);

                    anim.SetFloat("Velocity", dir.magnitude);

                    Debug.Log("Set Floats!");

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

                    taskEnded = GameTime.Now;
                    
                    // Set combatStart.
                    this.combatStart = GameTime.Now;

                    Debug.Log("Combat - Enter");

                    this.combatTask = null;

                    anim.SetInteger("inCombat", 1);
                    break;
                case FSMSubState.Remain:
                    //if ((this.combatTask != null && this.combatTask.Type != EntityTaskType.WatchdogCharge && this.combatTask.IsActive) || !taskEnded.HasTimeElapsed(1f))
                    //{
                        
                    //    this.combatStart = GameTime.Now;
                    //    break;
                    //}

                    float dist = this.transform.DistanceTo(entityTarget.transform);

                    // If in range to charge, then charge
                    if (dist < this.meleeMax)
                    {
                        if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.WatchdogCharge) && taskEnded.HasTimeElapsed(1f))
                        {
                            this.Owner.TaskManager.ClearTasks();
                            this.combatTask = new TaskWatchdogCharge(this.Owner, this.entityTarget, charge);
                            this.combatTask.Ended += CombatTask_Ended;
                            this.combatTask.Assign();
                        }
                    }
                    // If in range to blind, try to blind
                    else if (dist < this.EntityScanner.getDetectionRange())
                    {
                        if (this.blindTime == null || this.blindTime.HasTimeElapsed(this.blindingCooldown))
                        {
                            if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.Combat) && taskEnded.HasTimeElapsed(1f))
                            {
                                this.Owner.TaskManager.ClearTasks();
                                this.combatTask = new TaskCombat(this.Owner, this.entityTarget);
                                this.combatTask.Ended += CombatTask_Ended;
                                this.combatTask.Assign();
                                this.blindTime = GameTime.Now;
                            }
                        }
                        else // If blinding is on cooldown, then move towards the player
                        {
                            //if (!this.Owner.TaskManager.IsTaskActive(EntityTaskType.MoveToEntity))
                            //{
                            //    this.Owner.TaskManager.ClearTasks();
                            //    this.combatTask = new TaskMoveToEntity(this.Owner, this.entityTarget, this.meleeMax);
                            //    this.combatTask.Assign();
                            //}
                        }
                    }
                    else
                    {   
                        if (this.combatStart.HasTimeElapsed(alertTime))
                        {
                            sender.AdvanceTo(WatchdogStates.Patrol);
                        }
                    }

                    var dir = this.GetComponent<NavMeshAgent>().velocity.normalized;
                    anim.SetFloat("Horizontal", dir.x);

                    anim.SetFloat("Vertical", dir.y);

                    anim.SetFloat("Velocity", dir.magnitude);

                    Debug.Log("Set Floats!");

                    break;
                case FSMSubState.Leave:
                    Debug.Log("Leaving Combat!");
                    anim.SetInteger("inCombat", 0);

                    break;
            }
        }

        private void CombatTask_Ended(EntityTask task, bool successful)
        {
            taskEnded = GameTime.Now;
        }

        private bool NearTargets()
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit Player!!");
                collision.gameObject.GetComponent<PlayerEntity>().Kill();
            }
        }
    }
}
