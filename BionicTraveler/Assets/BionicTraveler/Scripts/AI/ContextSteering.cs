namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Tilemaps;

    /// <summary>
    /// Context based steering behavior for enemies. Partially based on
    /// http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter18_Context_Steering_Behavior-Driven_Steering_at_the_Macro_Scale.pdf
    /// https://www.ggdesign.me/goodreads/context-based-steering-4.
    /// </summary>
    public class ContextSteering : MonoBehaviour
    {
        [Header("Behaviour Setup")]
        [SerializeField]
        private float scanRadius;

        [SerializeField]
        private float avoidanceRadius;

        [SerializeField]
        private float currentAvoidanceRadius;

        [SerializeField]
        [Range(2, 30)]
        private int resolution;
        private float avoidAvoidanceWeight;
        private float dangerAvoidanceWeight;

        // TODO: Should be in something like EntityTemplate.
        [SerializeField]
        private bool stopWhenDying;

        [Header("Debug")]
        [SerializeField]
        private bool showDebugLines;

        [SerializeField]
        private bool dontMove;

        [Header("Weight Lists")]
        [SerializeField]
        private List<Vector3> detectionRays;

        [SerializeField]
        private List<float> interest;

        [SerializeField]
        private List<float> danger;

        [SerializeField]
        private List<float> avoid;

        [SerializeField]
        private List<float> combinedWeight;
        private float[] newWeights;

        private bool isInitialized;
        private NavMeshAgent agent;
        private Collider2D ourCollider;
        private bool wasTargetObstructedLastTick;
        private GameTime timeTargetUnobstructed;
        private GameTime lastPathfindingUpdate;
        private Vector3 lastDirection;

        // Stuck detection.
        private bool isAvoidingBadPath;
        private bool stuckOnX;
        private bool stuckOnY;
        private GameTime stuckLastX;
        private GameTime stuckLastY;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextSteering"/> class.
        /// </summary>
        public ContextSteering()
        {
            this.scanRadius = 5f;
            this.avoidanceRadius = 4f;
            this.resolution = 20;
            this.avoidAvoidanceWeight = -0.7f;
            this.dangerAvoidanceWeight = -0.8f;
            this.showDebugLines = false;
            this.dontMove = false;
        }

        private void Awake()
        {
            // TODO: Configure via ScriptableObject?
            this.ourCollider = this.GetComponent<Collider2D>();
            this.SetUpContexts();
            this.timeTargetUnobstructed = GameTime.Default;
            this.lastPathfindingUpdate = GameTime.Default;
            this.isInitialized = true;
        }

        private void Start()
        {
            this.agent = this.GetComponent<NavMeshAgent>();
            this.agent.updateRotation = false;
            this.agent.updateUpAxis = false;

            this.stuckLastX = GameTime.Now;
            this.stuckLastY = GameTime.Now;
        }

        /// <summary>
        /// Set up of internal lists for the contexts and the detection rays.
        /// </summary>
        private void SetUpContexts()
        {
            this.detectionRays = new List<Vector3>();
            this.interest = new List<float>();
            this.danger = new List<float>();
            this.avoid = new List<float>();
            this.combinedWeight = new List<float>();

            for (int i = 0; i < this.resolution; i++)
            {
                this.GetDetectionRayPoints(i, i + 1);
            }
        }

        /// <summary>
        /// Initalizes the detection rays and the context lists.
        /// </summary>
        private void GetDetectionRayPoints(int resolutionIndex, int currentDetectionRayCount)
        {
            // Divide the viewing angles into lines based on the resolution.
            float viewAngle = 360 / this.resolution;
            float viewAngleUpdated = viewAngle * resolutionIndex;
            var viewAngleDir = this.DirFromAngle(viewAngleUpdated, true);

            // Add detection ray point.
            if (this.detectionRays.Count < currentDetectionRayCount)
            {
                this.detectionRays.Add(viewAngleDir);
                this.interest.Add(0);
                this.danger.Add(0);
                this.avoid.Add(0);
                this.combinedWeight.Add(0);
            }
        }

        private void Update()
        {
            if (!this.agent.isActiveAndEnabled)
            {
                return;
            }

            // TODO: Have main AI class that determines target to follow and attack.
            var targetObj = GameObject.FindGameObjectWithTag("Player");
            var stopMovement = targetObj.GetComponent<PlayerEntity>().IsIgnoredByEveryone
                || (this.GetComponent<Entity>().IsDeadOrDying && this.stopWhenDying);
            if (stopMovement)
            {
                this.StopFollowing();
                return;
            }

            var target = targetObj.transform;
            var targetCollider = target.GetComponent<Collider2D>();
            var distanceToTarget = Vector3.Distance(this.ourCollider.bounds.center, targetCollider.bounds.center);

            // Stop chasing. TODO: Make customizable.
            if (distanceToTarget > 20)
            {
                this.StopFollowing();
                return;
            }

            if (distanceToTarget > 10)
            {
                this.UseNavmeshPathfinding(target);
                return;
            }

            // Ensure avoidance radius is smaller than target distance so that we can always reach our target.
            this.currentAvoidanceRadius = Math.Max(0, Math.Min(this.avoidanceRadius, distanceToTarget - 0.5f));

            // If we have no clear line to our target and are blocked by an environmental collider
            // defer to pathfinding. Local pathing does not work well to navigate complex environments.
            // For now anything we hit that is not a dynamic entity we consider a static obstacles.
            // A* is better suited to navigate those.
            // Also make sure that attacks do not count as otherwise projectiles make A* kick in.
            var targetDirection = (targetCollider.bounds.center - this.ourCollider.bounds.center).normalized;
            var directObstacles = this.ExecuteRaycast(true, this.transform, targetDirection, distanceToTarget);
            bool IsEnvironment(RaycastHit2D hit) => hit.collider.GetComponent<DynamicEntity>() == null
                && hit.collider.gameObject.tag != "Attack";
            var isObstructedByEnvironment = directObstacles.Any(IsEnvironment);

            if (isObstructedByEnvironment)
            {
                this.wasTargetObstructedLastTick = true;
            }
            else
            {
                if (this.wasTargetObstructedLastTick)
                {
                    this.wasTargetObstructedLastTick = false;
                    this.timeTargetUnobstructed = GameTime.Now;
                }
            }

            // Let A* take over.
            if (isObstructedByEnvironment || !this.timeTargetUnobstructed.HasTimeElapsed(1))
            {
                this.UseNavmeshPathfinding(target);
            }
            else
            {
                // Use local pathfinding and manipulate velocity manually.
                var context = this.Tick(target);
                var rayBasedDistance = directObstacles.FirstOrDefault(
                    obstacle => obstacle.collider.gameObject == targetObj);
                if (rayBasedDistance.distance > 1.0f)
                {
                    var velocity = (context.BestPoint - this.transform.position).normalized * 2;
                    this.agent.isStopped = true;
                    this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                    this.agent.velocity = this.dontMove ? Vector3.zero : this.FixUpVector(velocity);

                    var animator = this.GetComponent<Animator>();
                    if (animator != null)
                    {
                        var velocityInput = this.agent.velocity;
                        animator.SetInteger("MovementState", 1);
                        animator.SetFloat("Horizontal", velocityInput.x);
                        animator.SetFloat("Vertical", velocityInput.y);
                    }
                }
                else
                {
                    this.agent.isStopped = true;
                    this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                    this.agent.velocity = Vector3.zero;
                    this.GetComponent<Animator>()?.SetInteger("MovementState", 0);

                    //var animator = this.GetComponent<Animator>();
                    //animator.SetFloat("Horizontal", targetDirection.x);
                    //animator.SetFloat("Vertical", targetDirection.y);
                }
            }
        }

        private void StuckCheck(bool isStuck, ref bool stuckState, ref GameTime stuckTime, Func<Vector3> newPosition)
        {
            if (!isStuck)
            {
                stuckState = false;
                return;
            }

            if (!stuckState)
            {
                stuckTime = GameTime.Now;
                stuckState = true;
            }

            if (stuckTime.HasTimeElapsed(0.1f) && !this.isAvoidingBadPath)
            {
                stuckTime = GameTime.Now;
                this.agent.SetDestination(this.transform.position + newPosition());
                this.isAvoidingBadPath = true;
            }
        }

        private void UseNavmeshPathfinding(Transform target)
        {
            if (this.showDebugLines)
            {
                var path = this.agent.path;
                if (path != null)
                {
                    for (int i = 1; i < path.corners.Length; i++)
                    {
                        UI.DrawCircle(path.corners[i], 0.1f, Color.blue);
                        Debug.DrawLine(path.corners[i - 1], path.corners[i]);
                    }

                    UI.DrawCircle(this.agent.nextPosition, 0.3f, Color.red);
                }
            }

            // If desired velocity does not match actual velocity, we are either accelerating or stuck.
            // If our current velocity is zero for a while on one axis, we can assume that we are stuck.
            // Unity pathfinding gets stuck on NavMesh edges quite a bit so we have to detect it and steer
            // away from the edges to resume normal walking behavior.
            var desiredVelocity = this.agent.desiredVelocity;
            var actualVelocity = this.agent.velocity;
            var stuckOnX = Math.Abs(desiredVelocity.x - actualVelocity.x) > 0.2f && actualVelocity.x == 0;
            var stuckOnY = Math.Abs(desiredVelocity.y - actualVelocity.y) > 0.2f && actualVelocity.y == 0;

            this.StuckCheck(stuckOnX, ref stuckOnX, ref this.stuckLastX, () => new Vector3(Mathf.Clamp(desiredVelocity.x * -500, -1f, 1f), desiredVelocity.y, desiredVelocity.z));
            this.StuckCheck(stuckOnY, ref stuckOnY, ref this.stuckLastY, () => new Vector3(desiredVelocity.x, desiredVelocity.y + Mathf.Clamp(desiredVelocity.x * -500, -1f, 1f), desiredVelocity.z));

            var forceRepath = this.isAvoidingBadPath && this.agent.desiredVelocity.magnitude < 0.3f;
            if (this.lastPathfindingUpdate.HasTimeElapsed(0.5f) || forceRepath)
            {
                this.agent.isStopped = false;
                this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                this.agent.SetDestination(target.position);
                this.lastPathfindingUpdate = GameTime.Now;
                this.isAvoidingBadPath = false;
            }

            var animator = this.GetComponent<Animator>();
            if (animator != null)
            {
                var velocityInput = this.agent.velocity;
                animator.SetInteger("MovementState", 1);
                animator.SetFloat("Horizontal", velocityInput.x);
                animator.SetFloat("Vertical", velocityInput.y);
            }
        }

        private void StopFollowing()
        {
            this.agent.isStopped = true;
            this.agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            this.GetComponent<Animator>()?.SetInteger("MovementState", 0);
        }

        private Vector3 FixUpVector(Vector3 vector)
        {
            static float FixValue(float val) => Math.Abs(val) < 0.01f ? 0.01f : val;
            return new Vector3(FixValue(vector.x), FixValue(vector.y), FixValue(vector.z));
        }

        private ContextReturnData Tick(Transform target)
        {
            if (!this.isInitialized)
            {
                Debug.LogError("Context Steering is not initialized.  Initialize context steering before calling the Tick method");
                return new ContextReturnData(Vector3.zero, -1, 1f);
            }

            // Resizes the lists based on changes to the resolution.
            if (this.detectionRays.Count < this.resolution || this.detectionRays.Count > this.resolution)
            {
                this.SetUpContexts();
            }

            // First pass, build all the main weights,
            for (int i = 0; i < this.detectionRays.Count; i++)
            {
                this.CalculatePathWeights(this.detectionRays[i], i, target);
            }

            // Now use the three main layers to build the combined weights.
            this.newWeights = new float[this.resolution];
            for (int i = 0; i < this.detectionRays.Count; i++)
            {
                this.CalculatePathWeightsAvoidance(i, target);

                if (this.showDebugLines)
                {
                    this.DrawSteeringContextDebugLines(this.detectionRays[i], i);
                }
            }

            return this.ChooseBestPoint();
        }

        private void CalculatePathWeights(Vector3 detectionRayPoint, int detectionRayIndex, Transform target)
        {
            this.SetInterest(detectionRayPoint, detectionRayIndex, target);
            this.SetDanger(detectionRayIndex);
            this.SetAvoid(detectionRayIndex);
        }

        private void CalculatePathWeightsAvoidance(int detectionRayIndex, Transform target)
        {
            int oppositeIndex = (detectionRayIndex + (this.resolution / 2)) % this.resolution;
            if (this.avoid[oppositeIndex] < 0)
            {
                this.interest[detectionRayIndex] += -this.avoid[oppositeIndex];
            }

            this.newWeights[detectionRayIndex] = this.interest[detectionRayIndex];

            if (this.avoid[detectionRayIndex] < 0)
            {
                this.newWeights[detectionRayIndex] = 0;
            }

            // Blend new values with old weights to make direction changes less sudden.
            var diff = this.newWeights[detectionRayIndex] - this.combinedWeight[detectionRayIndex];
            this.combinedWeight[detectionRayIndex] += diff / 100;
        }

        private ContextReturnData ChooseBestPoint()
        {
            // Sum all vectors by their strength.
            var chosenDirection = Vector3.zero;
            for (int i = 0; i < this.resolution; i++)
            {
                chosenDirection += this.detectionRays[i] * Mathf.Clamp01(this.combinedWeight[i]);
            }

            chosenDirection.Normalize();

            if (this.showDebugLines)
            {
                Debug.DrawLine(this.transform.position, this.transform.position + (chosenDirection * 3), Color.blue);
            }

            chosenDirection = Vector3.Lerp(chosenDirection, this.lastDirection, 0.1f);

            this.lastDirection = chosenDirection;
            return new ContextReturnData(this.transform.position + chosenDirection, 0, 0);
        }

        private void SetInterest(Vector3 detectionRayPoint, int detectionRayIndex, Transform target)
        {
            this.interest[detectionRayIndex] = Mathf.Clamp01(this.CheckDotProductOfVector(detectionRayPoint, target));
        }

        private void SetAvoid(int detectionRayIndex)
        {
            var hitAvoid2D = this.CheckPathObstruction(this.detectionRays[detectionRayIndex]);
            if (hitAvoid2D.Length > 0)
            {
                foreach (RaycastHit2D hit2D in hitAvoid2D)
                {
                    if (hit2D.collider != this.ourCollider)
                    {
                        this.avoid[detectionRayIndex] = -Mathf.Clamp01(this.currentAvoidanceRadius - hit2D.distance);
                        if (-hit2D.distance > this.avoidAvoidanceWeight)
                        {
                            this.avoid[detectionRayIndex] = -1;
                        }

                        break;
                    }
                    else
                    {
                        this.avoid[detectionRayIndex] = 0;
                    }
                }
            }
            else
            {
                this.avoid[detectionRayIndex] = 0;
            }
        }

        private void SetDanger(int detectionRayIndex)
        {
            var hitDanger2D = this.CheckPathDanger(this.detectionRays[detectionRayIndex]);
            if (hitDanger2D.Length > 0)
            {
                foreach (RaycastHit2D hit2D in hitDanger2D)
                {
                    if (hit2D.collider != this.ourCollider)
                    {
                        this.danger[detectionRayIndex] = -Mathf.Clamp01(this.currentAvoidanceRadius - hit2D.distance);
                        if (-hit2D.distance > this.dangerAvoidanceWeight)
                        {
                            this.danger[detectionRayIndex] = -1;
                        }

                        break;
                    }
                    else
                    {
                        this.danger[detectionRayIndex] = 0;
                    }
                }
            }
            else
            {
                this.danger[detectionRayIndex] = 0;
            }
        }

        /// <summary>
        /// Create a raycast to detect the obstructions in the avoid mask.
        /// </summary>
        private RaycastHit2D[] CheckPathObstruction(Vector3 detectionRayPoint)
        {
            return this.ExecuteRaycast(true, this.transform, detectionRayPoint, this.scanRadius);
        }

        /// <summary>
        /// Create a raycast to detect the danagers in the dangers mask.
        /// </summary>
        private RaycastHit2D[] CheckPathDanger(Vector3 detectionRayPoint)
        {
            return this.ExecuteRaycast(false, this.transform, detectionRayPoint, this.scanRadius);
        }

        /// <summary>
        /// Scale a float within a new range.
        /// </summary>
        private float ScaleFloat(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
        {
            float oldRange = oldMax - oldMin;
            float newRange = newMax - newMin;
            float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;
            return newValue;
        }

        /// <summary>
        /// Used for visualizing the detection rays.
        /// </summary>
        private void DrawSteeringContextDebugLines(Vector3 detectionRayPos, int detectionRayIndex)
        {
            var combinedW = this.combinedWeight[detectionRayIndex];

            if (combinedW > 1 * 0.9)
            {
                Debug.DrawLine(
                    this.transform.position,
                    this.transform.position + (detectionRayPos.normalized * this.ScaleFloat(0, 1, 0, this.scanRadius, combinedW)),
                    Color.green);
            }
            else if (combinedW > 1 * 0.5)
            {
                Debug.DrawLine(
                    this.transform.position,
                    this.transform.position + (detectionRayPos.normalized * this.ScaleFloat(0, 1, 0, this.scanRadius, combinedW)),
                    Color.yellow);
            }
            else if (combinedW > 1 * 0.2)
            {
                Debug.DrawLine(
                    this.transform.position,
                    this.transform.position + (detectionRayPos.normalized * this.ScaleFloat(0, 1, 0, this.scanRadius, combinedW)),
                    Color.red);
            }
        }

        /// <summary>
        /// Calculate the direction based on an angle.
        /// </summary>
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += -this.transform.eulerAngles.z;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
        }

        /// <summary>
        /// Create a ray according to the directions passed and return the RayCastHit Array of all colliders hit.
        /// </summary>
        private RaycastHit2D[] ExecuteRaycast(bool avoidance, Transform transform, Vector3 dir, float scanDistance)
        {
            return Physics2D.RaycastAll(transform.position, dir, scanDistance);
        }

        /// <summary>
        /// Get the Dot product of the targetTransform and lines direction .
        /// Used to weight the interest of a specific direction.
        /// </summary>
        private float CheckDotProductOfVector(Vector3 detectionRayPos, Transform target)
        {
            return Vector3.Dot((target.position - this.transform.position).normalized, detectionRayPos.normalized);
        }

        private struct ContextReturnData
        {
            public Vector3 BestPoint;
            public int BestPointIndex;
            public float BestPointWeight;

            public ContextReturnData(Vector3 bestPoint, int bestPointIndex, float bestPointWeight)
            {
                this.BestPoint = bestPoint;
                this.BestPointIndex = bestPointIndex;
                this.BestPointWeight = bestPointWeight;
            }
        }
    }
}