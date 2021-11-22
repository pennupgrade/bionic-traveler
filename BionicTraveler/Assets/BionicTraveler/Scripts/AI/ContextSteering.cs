namespace BionicTraveler.Scripts.AI
{
    using System.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using Framework;
    using UnityEngine.Tilemaps;
    using BionicTraveler.Assets.Framework;

    public static class CollectionsExtensions
    {
        public static List<int> FindAllIndex<T>(this List<T> container, Predicate<T> match)
        {
            var items = container.FindAll(match);
            List<int> indexes = new List<int>();
            foreach (var item in items)
            {
                indexes.Add(container.IndexOf(item));
            }

            return indexes;
        }
    }

    public class ContextSteering : MonoBehaviour
    {
        [Header("Behaviour Setup")]
        //public SteeringBehaviour steeringBehaviour;

        private float circleRadius;
        private float avoidanceRadius;
        private float currentAvoidanceRadius;
        [Range(2, 30)]
        public int resolution;
        private float steeringSmoothingFactor;
        private float avoidAvoidanceWeight;
        private float danagerAvoidanceWeight;

        [Header("Mask Filters")]
        private LayerMask interestLayermask;
        private LayerMask avoidLayerMask;
        private LayerMask dangerLayermask;

        [Header("Debug")]
        private bool showDebugLines;

        //Local Vars
        private bool isInitialized;
        private Rigidbody2D myRigidBody;
        private Collider2D myCollider;

        #region Weight Lists
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

        [SerializeField]
        private bool dontMove;

        private bool wasTargetObstructedLastTick;
        private GameTime timeTargetUnobstructed;
        #endregion

        //All set up methods for the class.
        #region Initialization Methods
        private void Awake()
        {
            this.circleRadius = 5f; // steeringBehaviour.circleRadius;
            this.avoidanceRadius = 4f;
            this.resolution = 20; // steeringBehaviour.resolution;
            this.avoidAvoidanceWeight = -0.7f; // steeringBehaviour.avoidAvoidanceWeight;
            this.danagerAvoidanceWeight = -0.8f; // steeringBehaviour.danagerAvoidanceWeight;
            //this.interestLayermask = steeringBehaviour.interestLayermask;
            //this.avoidLayerMask = steeringBehaviour.avoidLayerMask;
            //this.dangerLayermask = steeringBehaviour.dangerLayermask;
            this.showDebugLines = true; // steeringBehaviour.showDebugLines;

            this.myCollider = GetComponent<Collider2D>();
            this.myRigidBody = GetComponent<Rigidbody2D>();

            SetUpContexts();

            this.timeTargetUnobstructed = GameTime.Default;
            this.isInitialized = true;
        }

        /// <summary>
        /// Set up of internal lists for the contexts and the detection rays
        /// </summary>
        private void SetUpContexts()
        {
            detectionRays = new List<Vector3>();
            interest = new List<float>();
            danger = new List<float>();
            avoid = new List<float>();
            combinedWeight = new List<float>();

            for (int i = 0; i < resolution; i++)
            {
                GetDetectionRayPoints(i, i + 1);
            }
        }

        /// <summary>
        /// Initalizes the detection rays and the context lists 
        /// </summary>
        private void GetDetectionRayPoints(int resolutionIndex, int currentDetectionRayCount)
        {
            //Divide the viewing angles into lines based on the resolution
            float viewAngle = 360 / resolution;
            //Store a ref to the view angle of each line
            Vector3 viewAngleDir;
            //calculate the direction of the line around the circle
            float viewAngleUpdated = viewAngle * resolutionIndex;
            //Calculate the angle of the line around the circle
            viewAngleDir = DirFromAngle(viewAngleUpdated, true);
            //Add detection ray point
            if (detectionRays.Count < currentDetectionRayCount)
            {
                detectionRays.Add(viewAngleDir);
                interest.Add(0);
                danger.Add(0);
                avoid.Add(0);
                combinedWeight.Add(0);
            }
        }
        #endregion

        /// <summary>
        /// Can be overridden.  Used as the Conext Behaviours Update function and meant to be 
        /// called from a central scripts update function
        /// </summary>
        public void Update()
        {
            var target = GameObject.FindGameObjectWithTag("Player").transform;
            var distanceToTarget = this.transform.DistanceTo(target);
            var agent = this.GetComponent<NavMeshAgent>();

            // Ensure avoidance radius is smaller than target distance so that we can always reach our target.
            this.currentAvoidanceRadius = Math.Max(0, Math.Min(this.avoidanceRadius, distanceToTarget - 0.5f));

            // If we have no clear line to our target and are blocked by an environmental collider
            // defer to pathfinding.
            var targetDirection = (target.position - this.transform.position).normalized;
            var directObstacles = this.CreateObjectDetectionRay2D_ALL(true, this.transform, targetDirection, distanceToTarget);
            var isObstructedByEnvironment = directObstacles.Any(obstable => obstable.collider.GetComponent<TilemapCollider2D>());

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

            if (isObstructedByEnvironment || !this.timeTargetUnobstructed.HasTimeElapsed(1))
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
            else
            {
                var context = this.Tick(target);

                if (Vector3.Distance(this.transform.position, target.position) > 0.5f)
                {
                    //Move
                    var velocity = (context.bestPoint - this.transform.position).normalized * 2;
                    agent.velocity = this.dontMove ? Vector3.zero : this.FixUpVector(velocity);
                    //this.transform.position += (context.bestPoint - this.transform.position).normalized * 4 * Time.deltaTime;

                    var animator = this.GetComponent<Animator>();
                    var velocityInput = agent.velocity;
                    animator.SetFloat("Horizontal", velocityInput.x);
                    animator.SetFloat("Vertical", velocityInput.y);
                    //RotateGameObject(contextReturnData.bestPoint, rotationSpeed, rotationOffset);
                }
            }
        }

        private Vector3 FixUpVector(Vector3 vector)
        {
            float x = vector.x, y = vector.y, z = vector.z;
            if (Math.Abs(x) < 0.01f) x = 0.01f;
            if (Math.Abs(y) < 0.01f) y = 0.01f;
            if (Math.Abs(z) < 0.01f) z = 0.01f;
            return new Vector3(x, y, z);
        }

        private ContextReturnData Tick(Transform target)
        {
            if (!isInitialized)
            {
                Debug.LogError("Context Steering is not initialized.  Initialize context steering before calling the Tick method");
                return new ContextReturnData(Vector3.zero, -1, 1f);
            }

            //Resizes the lists based on changes to the resolution
            if (detectionRays.Count < resolution || detectionRays.Count > resolution)
            {
                SetUpContexts();
            }

            //Draws the detection rays
            bool anyAvoidance = false;
            this.newWeights = new float[this.resolution];
            for (int i = 0; i < detectionRays.Count; i++)
            {
                CalculatePathWeights(detectionRays[i], i, target);
                if (avoid[i] == -1 || danger[i] == -1)
                {
                    anyAvoidance = true;
                }
            }

            for (int i = 0; i < detectionRays.Count; i++)
            {
                if (anyAvoidance)
                {
                    //combinedWeight[i] = Mathf.Clamp(combinedWeight[i], 0, 0.8f);
                }

                CalculatePathWeightsAvoidance(detectionRays[i], i, target);

                if (showDebugLines)
                {
                    DrawSteeringContextDebugLines(detectionRays[i], i);
                }
            }

            return ChooseBestPoint();
        }

        #region Path Calculation
        private void CalculatePathWeights(Vector3 detectionRayPoint, int detectionRayIndex, Transform target)
        {
            SetInterest(detectionRayPoint, detectionRayIndex, target);
            SetDanger(detectionRayPoint, detectionRayIndex);
            SetAvoid(detectionRayPoint, detectionRayIndex);

            //combinedWeight[detectionRayIndex] =
            //                                Mathf.Clamp01(interest[detectionRayIndex] +
            //                                danger[detectionRayIndex] +
            //                                avoid[detectionRayIndex]);
        }


        private void CalculatePathWeightsAvoidance(Vector3 detectionRayPoint, int detectionRayIndex, Transform target)
        {
            int oppositeIndex = (detectionRayIndex + (this.resolution / 2)) % this.resolution;
            if (avoid[oppositeIndex] < 0)
            {
                interest[detectionRayIndex] += -(avoid[oppositeIndex]);
            }

            newWeights[detectionRayIndex] = interest[detectionRayIndex];

            if (avoid[detectionRayIndex] < 0)
            {
                newWeights[detectionRayIndex] = 0;
            }

            // Blend new values with old weights.
            var diff = newWeights[detectionRayIndex] - combinedWeight[detectionRayIndex];
            combinedWeight[detectionRayIndex] += (diff / 100);
            if (combinedWeight[detectionRayIndex] < 0.1f)
            {
                //combinedWeight[detectionRayIndex] = 0;
            }

            //combinedWeight[detectionRayIndex] =
            //                                Mathf.Clamp01(interest[detectionRayIndex] +
            //                                danger[detectionRayIndex] +
            //                                avoid[detectionRayIndex]);
        }

        int bestIndex;
        float bestIndexWeight;
        Vector3 lastDirection;

        private ContextReturnData ChooseBestPoint()
        {
            var chosenDirection = Vector3.zero;
            for (int i = 0; i < this.resolution; i++)
            {
                chosenDirection += this.detectionRays[i] * Mathf.Clamp01(combinedWeight[i]);
            }

            chosenDirection.Normalize();

            Debug.DrawLine(
                  transform.position,
                  transform.position + (chosenDirection * 3),
                  Color.blue);
            chosenDirection = Vector3.Lerp(chosenDirection, lastDirection, 0.1f);

            lastDirection = chosenDirection;

            return new ContextReturnData(transform.position + chosenDirection, 0, 0);

            //Get the index with the best weight
            bestIndex = combinedWeight.IndexOf(combinedWeight.Max());
            return new ContextReturnData(WeightDirectionVectorsUsingCombinedWeights(bestIndex), bestIndex, combinedWeight[bestIndex]);

            //Get the index with the best weight
            bestIndex = combinedWeight.IndexOf(combinedWeight.Max());
            int leftDirIndex = GetDirToLeft(bestIndex);
            int rightDirIndex = GetDirToRight(bestIndex);
            var forwardMovement = (
                          (detectionRays[bestIndex] * combinedWeight[bestIndex]) +
                          (detectionRays[leftDirIndex] * combinedWeight[leftDirIndex]) +
                          (detectionRays[rightDirIndex] * combinedWeight[rightDirIndex])
                      ).normalized * ScaleFloat(0, 1, 0, circleRadius, combinedWeight[bestIndex]);

            //WeightDirectionVectorsUsingCombinedWeights(bestIndex);
            var awayIndex = avoid.FindAllIndex(f => f == avoid.Min());
            var awayMovement = Vector3.zero;
            foreach (var index in awayIndex)
            {
                var v = (detectionRays[index] * avoid[index]);
                awayMovement += v;
            }

            var combinedMovement = forwardMovement + (awayMovement * 5);
            var movementPoint = transform.position + combinedMovement;

            Debug.DrawLine(
                   transform.position,
                   transform.position + (awayMovement.normalized * 2),
                   Color.black);

            var finalMovement = movementPoint;

            if (showDebugLines)
            {
                Debug.DrawLine(
                        transform.position,
                        finalMovement,
                        Color.blue);
            }

            return new ContextReturnData(finalMovement, bestIndex, combinedWeight[bestIndex]);
        }

        Vector3 blendedDir;
        private Vector3 WeightDirectionVectorsUsingCombinedWeights(int bestCombinedWeightIndex)
        {
            int leftDirIndex = GetDirToLeft(bestCombinedWeightIndex);
            int rightDirIndex = GetDirToRight(bestCombinedWeightIndex);

            blendedDir =
                  transform.position +
                  (
                      (
                          (detectionRays[bestCombinedWeightIndex] * combinedWeight[bestCombinedWeightIndex]) +
                          (detectionRays[leftDirIndex] * combinedWeight[leftDirIndex]) +
                          (detectionRays[rightDirIndex] * combinedWeight[rightDirIndex])
                      ).normalized * ScaleFloat(0, 1, 0, circleRadius, combinedWeight[bestCombinedWeightIndex])
                  );

            if (showDebugLines)
            {
                Debug.DrawLine(
                        transform.position,
                        blendedDir,
                        Color.blue);
            }

            return blendedDir;
        }
        #endregion

        #region Set the Weights
        private void SetInterest(Vector3 detectionRayPoint, int detectionRayIndex, Transform target)
        {
            interest[detectionRayIndex] = Mathf.Clamp01(CheckDotProductOfVector(detectionRayPoint, target));
            //if (interest[detectionRayIndex] <= 0.3f)
            //{
            //    interest[detectionRayIndex] = 0.3f;
            //}
        }


        RaycastHit2D[] hitAvoid2D;
        private void SetAvoid(Vector3 detectionRayPoint, int detectionRayIndex)
        {
            hitAvoid2D = CheckPathObstruction(detectionRays[detectionRayIndex]);

            if (hitAvoid2D.Length > 0)
            {
                foreach (RaycastHit2D hit2D in hitAvoid2D)
                {
                    if (hit2D.collider != myCollider)
                    {
                        //Debug.Log(hit2D.collider.gameObject.name);
                        avoid[detectionRayIndex] = -Mathf.Clamp01(currentAvoidanceRadius - hit2D.distance);
                        if (-hit2D.distance > avoidAvoidanceWeight)
                        {
                            avoid[detectionRayIndex] = -1;
                        }
                        break;
                    }
                    else
                    {
                        avoid[detectionRayIndex] = 0;  //This sets the weight of this list index to 0 if no correct hit was found
                    }
                }
            }
            else
            {
                avoid[detectionRayIndex] = 0;
            }
        }


        RaycastHit2D[] hitDanager2D;
        /// <summary>
        /// Create a raycast to detect the obstructions in the avoid mask
        /// </summary>

        private void SetDanger(Vector3 detectionRayPoint, int detectionRayIndex)
        {
            //var obj = GameObject.Find("BFF (2)");
            //var distance = (obj.transform.DistanceTo(this.transform));
            //if (distance < this.circleRadius)
            //{
            //    danger[detectionRayIndex] = -Mathf.Clamp01(CheckDotProductOfVector(detectionRayPoint, obj.transform));
            //}
            //else
            //{
            //    danger[detectionRayIndex] = 0;
            //}

            //return;

            hitDanager2D = CheckPathDanger(detectionRays[detectionRayIndex]);

            if (hitDanager2D.Length > 0)
            {
                foreach (RaycastHit2D hit2D in hitDanager2D)
                {
                    if (hit2D.collider != myCollider)
                    {
                        danger[detectionRayIndex] = -Mathf.Clamp01(currentAvoidanceRadius - hit2D.distance);
                        if (-hit2D.distance > danagerAvoidanceWeight)
                        {
                            danger[detectionRayIndex] = -1;
                        }
                        break;
                    }
                    else
                    {
                        danger[detectionRayIndex] = 0;
                    }
                }
            }
            else
            {
                danger[detectionRayIndex] = 0;
            }
        }

        /// <summary>
        /// Create a raycast to detect the obstructions in the avoid mask
        /// </summary>
        private RaycastHit2D[] CheckPathObstruction(Vector3 detectionRayPoint)
        {
            return CreateObjectDetectionRay2D_ALL(
                true,
                transform,
                detectionRayPoint,
                circleRadius
            );
        }

        /// <summary>
        /// Create a raycast to detect the danagers in the dangers mask
        /// </summary>
        private RaycastHit2D[] CheckPathDanger(Vector3 detectionRayPoint)
        {
            return CreateObjectDetectionRay2D_ALL(
                false,
                transform,
                detectionRayPoint,
                circleRadius
            );
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Gets the detection ray index to the right of the best direction ray
        /// </summary>
        private int GetDirToRight(int index)
        {
            return (index == resolution - 1) ? 0 : index + 1;
        }

        /// <summary>
        /// Gets the detection ray index to the left of the passed direction ray with index n
        /// </summary>
        private int GetDirToLeft(int index)
        {
            return ((index - 1) < 0) ? resolution - Mathf.Abs(index - 1) : index - 1;
        }

        /// <summary>
        /// Scale a float within a new range
        /// </summary>
        private float ScaleFloat(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
        {

            float OldRange = (OldMax - OldMin);
            float NewRange = (NewMax - NewMin);
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

            return (NewValue);
        }

        float combinedW;

        /// <summary>
        /// Used for visualizing the detection rays
        /// </summary>
        private void DrawSteeringContextDebugLines(Vector3 detectionRayPos, int detectionRayIndex)
        {
            combinedW = combinedWeight[detectionRayIndex];

            if (combinedW > 1 * 0.9)
            {
                Debug.DrawLine(
                    transform.position,
                    transform.position + (detectionRayPos.normalized * ScaleFloat(0, 1, 0, circleRadius, combinedW)),
                    Color.green);
            }
            else if (combinedW > 1 * 0.5)
            {
                Debug.DrawLine(
                    transform.position,
                    transform.position + (detectionRayPos.normalized * ScaleFloat(0, 1, 0, circleRadius, combinedW)),
                    Color.yellow);
            }
            else if (combinedW > 1 * 0.2)
            {
                Debug.DrawLine(
                    transform.position,
                    transform.position + (detectionRayPos.normalized * ScaleFloat(0, 1, 0, circleRadius, combinedW)),
                    Color.red);
            }
        }

        /// <summary>
        /// Calculate the direction based on an angle
        /// </summary>
        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += -transform.eulerAngles.z;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
        }

        /// <summary>
        /// Create a ray according to the directions passed and return the RayCastHit Array of all colliders hit
        /// </summary>
        private RaycastHit2D[] CreateObjectDetectionRay2D_ALL(bool avoidance, Transform transform, Vector3 dir, float scanDistance)
        {
            // TODO: Danger/Avoidance filter.
            var hits = Physics2D.RaycastAll(transform.position, dir, scanDistance);
            return hits;

            if (avoidance)
            {
                hits = hits.Where(hit => hit.collider != null && hit.collider.gameObject.CompareTag("Enemy")).ToArray();
            }
            else
            {

                hits = hits.Where(hit => hit.collider != null && hit.collider.gameObject.CompareTag("Enemy")).ToArray();
            }

            return hits;
        }

        /// <summary>
        /// Get the Dot product of the targetTransform and lines direction 
        /// Used to weight the interest of a specific direction
        /// </summary>
        private float CheckDotProductOfVector(Vector3 detectionRayPos, Transform target)
        {
            if (showDebugLines)
            {
                //Debug.Log("Target Pos: " + target.position + " Target Transform:        " + target);
            }

            return Vector3.Dot((target.position - this.transform.position).normalized, detectionRayPos.normalized);
        }
        #endregion
    }

    public struct ContextReturnData
    {
        public Vector3 bestPoint;
        public int bestPointIndex;
        public float bestPointWeight;

        public ContextReturnData(Vector3 bestPoint, int bestPointIndex, float bestPointWeight)
        {
            this.bestPoint = bestPoint;
            this.bestPointIndex = bestPointIndex;
            this.bestPointWeight = bestPointWeight;
        }
    }
}