namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// The slow trap enemy behavior.
    /// </summary>
    public class HideChaseEnemyBehavior : EntityBehavior
    {
        public enum HideChaseEntityGoal
        {
            Hide,
            Chase
        }

        private Entity entityTarget;
        private float minX, maxX;
        private float minY, maxY;

        private GameTime lastShot;
        private GameTime trapStart;

        public GameObject fishPrefab;

        [SerializeField]
        private AttackData attackData;

        public override void Awake()
        {
            base.Awake();

            this.minX = this.StartPosition.x + 5f;
            this.maxX = this.StartPosition.x - 5f;
            this.minY = this.StartPosition.y + 5f;
            this.maxY = this.StartPosition.y - 5f;
        }

        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<HideChaseEntityGoal>();
            fsm.SetDefaultState(HideChaseEntityGoal.Hide);
            fsm.RegisterCallback(HideChaseEntityGoal.Hide, this.HideMode);
            fsm.RegisterCallback(HideChaseEntityGoal.Chase, this.ChaseMode);
            return fsm;
        }

        /// <summary>
        /// Stay in hide mode until player is detected within radius.
        /// </summary>
        private void HideMode(FSM<HideChaseEntityGoal> sender, HideChaseEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();
                    this.GetComponent<Animator>().Play("Idle");

                    break;
                case FSMSubState.Remain:
                    // Stays in state until targe entered range.
                    // Scans for target
                    if (this.CheckForNearbyTargets())
                    {
                        sender.AdvanceTo(HideChaseEntityGoal.Chase);
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private void ChaseMode(FSM<HideChaseEntityGoal> sender, HideChaseEntityGoal currentState, FSMSubState subState)
        {
            switch (subState)
            {
                case FSMSubState.Enter:
                    this.Owner.TaskManager.ClearTasks();

                    this.GetComponent<Animator>().Play("Leave");
                    //this.Shoot();
                    this.trapStart = GameTime.Now;

                    break;
                case FSMSubState.Remain:

                    if (this.GetComponent<Animator>().IsAnimationPlaying("Leave"))
                    {
                        this.GetComponent<Animator>().Play("Idle");

                        // Spawn the fish.
                        var fish = Instantiate(this.fishPrefab, this.transform.position, this.transform.rotation);
                    }

                    if (this.trapStart.HasTimeElapsed(10f))
                    {
                        sender.AdvanceTo(HideChaseEntityGoal.Hide);
                    }

                    //if (this.lastShot.HasTimeElapsedReset(2f))
                    //{
                    //    this.Shoot();
                    //}

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private void Shoot()
        {
            Vector3 targetPos = this.entityTarget.transform.position;
            Vector3 direction = (targetPos - this.transform.position).normalized;

            var attack = AttackFactory.CreateAttack(this.attackData);
            attack.StartAttack(this.Owner);

            Rigidbody2D rb1 = attack.GetComponent<Rigidbody2D>();
            rb1.velocity = 5 * direction;
            this.lastShot = GameTime.Now;
        }

        // Helper functions.
        private bool CheckForNearbyTargets()
        {
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
    }
}
