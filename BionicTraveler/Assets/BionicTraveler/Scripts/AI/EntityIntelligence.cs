namespace BionicTraveler.Scripts.AI
{
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Main AI class for entities to manage their behavior. Eventually we may want to split this up
    /// into decision making classes and task (sub)trees with a proper FSM once/if we want more nuanced behavior.
    /// </summary>
    public class EntityIntelligence : MonoBehaviour
    {
        // TODO: As indicated by the top comment, we may want to move these into specific subbehaviors at
        // some point.
        [SerializeField]
        [Tooltip("The range when an entity will attack a hostile target.")]
        private float combatRange;

        [SerializeField]
        [Tooltip("The range to patrol.")]
        private float patrolRange;

        //[SerializeField]
        //[Tooltip("The default behavior state.")]
        //private EntityGoal defaultState;

        private DynamicEntity owner;
        private EntityMovement movement;
        private EnemyCombatBehaviour combatBehaviour;
        private EntityBehavior mainBehavior;

        private void Awake()
        {
            this.owner = this.GetComponent<DynamicEntity>();
            this.movement = this.GetComponent<EntityMovement>();
            this.combatBehaviour = this.GetComponent<EnemyCombatBehaviour>();
            this.mainBehavior = this.GetComponent<EntityBehavior>();
        }

        public float CombatRange => this.combatRange;

        //private void Start()
        //{
        //    // Default state is idle.
        //    this.TransitionToDefaultState();
        //}

        //private void Update()
        //{
        //    if (this.lastGoalUpdate.HasTimeElapsedReset(0.1f))
        //    {
        //        this.ProcessGoal();
        //    }
        //}

        //private void ProcessGoal()
        //{
        //    switch (this.primaryGoal)
        //    {
        //        case EntityGoal.Idle:
        //            this.ProcessIdle();
        //            break;

        //        case EntityGoal.Patrol:
        //            this.ProcessPatrol();
        //            break;

        //        case EntityGoal.Combat:
        //            this.ProcessCombat();
        //            break;
        //    }
        //}

        //private void TransitionToDefaultState()
        //{
        //    switch (this.defaultState)
        //    {
        //        case EntityGoal.Idle:
        //            this.TransitionToIdle();
        //            break;
        //        case EntityGoal.Patrol:
        //            this.TransitionToPatrol();
        //            break;
        //        default:
        //            throw new System.InvalidOperationException("Invalid default state");
        //    }
        //}

        //private void TransitionToIdle()
        //{
        //    // No movement when idle.
        //    this.movement.ClearTarget();
        //    this.movement.ForceWalking = false;
        //    this.primaryGoal = EntityGoal.Idle;
        //}

        //private void ProcessIdle()
        //{
        //    if (!this.flags.HasFlag(EntityFlags.DontAttackWhenIdle))
        //    {
        //        this.CheckForNearbyTargets();
        //    }
        //}

        //private void CheckForNearbyTargets()
        //{
        //    // If we have a combat behavior, attack close targets. TODO: Configure via flags
        //    // such as attack targets on sight or not. Also add LOS checks.
        //    // TODO: Prioritize closest target.
        //    if (this.combatBehaviour != null && this.entityScanner != null)
        //    {
        //        var nearbyTargets = this.entityScanner.GetAllDynamicInRange();
        //        var target = nearbyTargets.FirstOrDefault();
        //        if (target != null && this.IsValidTarget(target))
        //        {
        //            if (this.flags.HasFlag(EntityFlags.AttackOnSight))
        //            {
        //                this.combatTarget = target;
        //                this.TransitionToCombat();
        //            }
        //        }
        //    }
        //}

        //private void TransitionToPatrol()
        //{
        //    this.primaryGoal = EntityGoal.Patrol;
        //    this.movement.ForceWalking = true;
        //    this.PatrolGoToNewPoint();
        //}

        //private void ProcessPatrol()
        //{
        //    this.CheckForNearbyTargets();
        //    if (this.movement.HasReached)
        //    {
        //        this.PatrolGoToNewPoint();
        //    }
        //}

        //private void PatrolGoToNewPoint()
        //{
        //    Vector3 position = this.patrolOrigin + (Random.insideUnitCircle * this.patrolRange).ToVector3();
        //    this.movement.SetTarget(position);
        //    Debug.Log("EntityIntelligence::PatrolGoToNewPoint: Set new waypoint");
        //}

        //private void TransitionToCombat()
        //{
        //    this.primaryGoal = EntityGoal.Combat;
        //    this.movement.ForceWalking = false;
        //}

        //private void ProcessCombat()
        //{
        //    if (!this.IsValidTarget(this.combatTarget))
        //    {
        //        this.combatTarget = null;
        //        this.TransitionToDefaultState();
        //    } 
        //    else
        //    {
        //        if (this.flags.HasFlag(EntityFlags.IsRangedAttacker))
        //        {
        //        this.movement.SetTarget(this.combatTarget.gameObject.transform.position + new Vector3(0,5,0));
        //        } 
        //        else 
        //        {
        //        this.movement.SetTarget(this.combatTarget.gameObject);
        //        }

        //        this.combatBehaviour.SetTarget(this.combatTarget);   
        //    }
        //}

        private bool IsValidTarget(Entity target)
        {
            var ignoreTarget = target.IsPlayer && ((PlayerEntity)target).IsIgnoredByEveryone;
            if (ignoreTarget)
            {
                return false;
            }

            if (!this.owner.Relationships.IsHostile(target.tag))
            {
                return false;
            }

            if (Vector3.Distance(this.transform.position, target.transform.position) > this.combatRange)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Draws a sphere showing the combat area.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, this.combatRange);
        }
    }
}
