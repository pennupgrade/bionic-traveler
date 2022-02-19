namespace BionicTraveler.Scripts.AI.HackathonT1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// BUG: If many are placed in close proximity, it will not detect the player.
    /// </summary>
    public class SlowTrapEnemyBehavior : EntityBehavior
    {
        public enum SlowTrapEntityGoal
        {
            Hide,
            Trap
        }

        private Entity entityTarget;
        private float minlen, maxlen;
        private float minX, maxX;
        private float minY, maxY;

        private IFSM fsm;
        private GameTime lastShot;
        private GameTime trapStart;

        public GameObject stickyTrapPod;

        public override void Awake()
        {
            base.Awake();

            //this.minlen = 5;
            //this.maxlen = 10;
            this.minX = this.StartPosition.x + 5f;
            this.maxX = this.StartPosition.x - 5f;
            this.minY = this.StartPosition.y + 5f;
            this.maxY = this.StartPosition.y - 5f;
        }

        public override IFSM CreateFSM()
        {
            var fsm = new FSM<SlowTrapEntityGoal>();
            fsm.SetDefaultState(SlowTrapEntityGoal.Hide);
            fsm.RegisterCallback(SlowTrapEntityGoal.Hide, this.HideMode);
            fsm.RegisterCallback(SlowTrapEntityGoal.Trap, this.TrapMode);
            return fsm;
        }

        /// <summary>
        /// Stay in hide mode until player is detected within radius
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentState"></param>
        /// <param name="subState"></param>
        private void HideMode(FSM<SlowTrapEntityGoal> sender, SlowTrapEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();

                    break;
                case FSMSubState.Remain:
                    // Stays in state until targe entered range.

                    // Scans for target
                    if (this.CheckForNearbyTargets())
                    {
                        sender.AdvanceTo(SlowTrapEntityGoal.Trap);
                    }
                    //Debug.Log("Target Spotted");
                    break;
                case FSMSubState.Leave:
                    //Debug.Log("Hide - Leave");
                    break;
            }
        }

        private void TrapMode(FSM<SlowTrapEntityGoal> sender, SlowTrapEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();

                    this.GetComponent<Animator>().Play("Attack");

                    Vector3 targetPos = this.entityTarget.transform.position;
                    float targetX = UnityEngine.Random.Range(minX, maxX);
                    float targetY = UnityEngine.Random.Range(minY, maxY);

                    Vector3 direction = (targetPos - this.transform.position).normalized;

                    GameObject trapPod1 = Instantiate(this.stickyTrapPod, new Vector3(StartPosition.x, StartPosition.y, 0), Quaternion.identity) as GameObject;
                    
                    TrapProjectile pod1 = trapPod1.GetComponent<TrapProjectile>();
                    Rigidbody2D rb1 = trapPod1.GetComponent<Rigidbody2D>();
                    
                    pod1.origin = this.StartPosition;
                    rb1.velocity = 5 * direction;


                    this.lastShot = GameTime.Now;
                    this.trapStart = GameTime.Now;

                    break;
                case FSMSubState.Remain:

                    if (this.trapStart.HasTimeElapsed(10f))
                    {
                        sender.AdvanceTo(SlowTrapEntityGoal.Hide);
                    }

                    // TODO: shoot periodically
                    if (this.lastShot.HasTimeElapsed(2f))
                    {

                        Vector3 targetPos2 = this.entityTarget.transform.position;
                        float targetX2 = UnityEngine.Random.Range(minX, maxX);
                        float targetY2 = UnityEngine.Random.Range(minY, maxY);

                        Vector3 direction2 = (targetPos2 - this.transform.position).normalized;
                        GameObject trapPod2 = Instantiate(this.stickyTrapPod, new Vector3(StartPosition.x, StartPosition.y, 0), Quaternion.identity) as GameObject;

                        TrapProjectile pod2 = trapPod2.GetComponent<TrapProjectile>();
                        Rigidbody2D rb2 = trapPod2.GetComponent<Rigidbody2D>();

                        pod2.origin = this.StartPosition;
                        rb2.velocity = 5 * direction2;

                        this.lastShot = GameTime.Now;
                    }
                    break;
                case FSMSubState.Leave:
                    //Debug.Log("Combat - Leave");
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
