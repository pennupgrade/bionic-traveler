namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Audio;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// The slow trap enemy behavior.
    /// </summary>
    public class SlowTrapEnemyBehavior : EntityBehavior
    {
        public enum SlowTrapEntityGoal
        {
            Hide,
            Trap
        }

        private Entity entityTarget;
        private float minX, maxX;
        private float minY, maxY;

        private GameTime lastShot;
        private GameTime trapStart;

        [SerializeField]
        private AudioClip openSound;

        [SerializeField]
        private AudioClip closeSound;

        [SerializeField]
        private AudioClip shootSound;

        [SerializeField]
        private AudioClip deathSound;

        public GameObject stickyTrapPod;

        [SerializeField]
        private AttackData attackData;

        public void PlayDeathSound(Entity sender, Entity killer)
        {
            AudioManager.Instance.PlayOneShot(deathSound);
            this.Owner.Dying -= this.PlayDeathSound;
        }

        public override void Awake()
        {
            base.Awake();

            this.Owner.Dying += this.PlayDeathSound;

            this.minX = this.StartPosition.x + 5f;
            this.maxX = this.StartPosition.x - 5f;
            this.minY = this.StartPosition.y + 5f;
            this.maxY = this.StartPosition.y - 5f;
        }
        /// <inheritdoc/>
        public override IFSM CreateFSM()
        {
            var fsm = new FSM<SlowTrapEntityGoal>();
            fsm.SetDefaultState(SlowTrapEntityGoal.Hide);
            fsm.RegisterCallback(SlowTrapEntityGoal.Hide, this.HideMode);
            fsm.RegisterCallback(SlowTrapEntityGoal.Trap, this.TrapMode);
            return fsm;
        }

        /// <summary>
        /// Stay in hide mode until player is detected within radius.
        /// </summary>
        private void HideMode(FSM<SlowTrapEntityGoal> sender, SlowTrapEntityGoal currentState, FSMSubState subState)
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
                        AudioManager.Instance.PlayOneShot(openSound);
                        sender.AdvanceTo(SlowTrapEntityGoal.Trap);
                    }

                    break;
                case FSMSubState.Leave:
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
                    
                    this.Shoot();
                    this.trapStart = GameTime.Now;

                    break;
                case FSMSubState.Remain:

                    if (this.trapStart.HasTimeElapsed(10f))
                    {
                        AudioManager.Instance.PlayOneShot(closeSound);
                        sender.AdvanceTo(SlowTrapEntityGoal.Hide);
                    }

                    if (this.lastShot.HasTimeElapsedReset(2f))
                    {
                        this.Shoot();
                    }

                    break;
                case FSMSubState.Leave:
                    break;
            }
        }

        private void Shoot()
        {
            AudioManager.Instance.PlayOneShot(shootSound);
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
