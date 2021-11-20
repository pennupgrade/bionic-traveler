namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Assets.Framework;
    using Framework;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Tilemaps;

    class Movement : MonoBehaviour
    {
        private const int NoDirections = 18;

        private MovementDirection[] directions;
        private Vector3 objectivePosition;
        private GameTime lastPositionUpdate;
        private Vector3 currentPosition;
        private Vector3 lastVelocity;

        public Movement()
        {
            this.directions = new MovementDirection[Movement.NoDirections];
        }

        private void Start()
        {
            this.lastPositionUpdate = GameTime.Now;
        }

        public void SetObjective(Vector3 position)
        {
            this.objectivePosition = position;
        }

        public void AddAvoidance(Vector3 position)
        {

        }

        private void Update()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            this.SetObjective(player.transform.position);

            // TODO: Only use when really close.
            var ourPos = this.transform.position;
            Vector2 desiredDirection = (this.objectivePosition - ourPos).ToVector2().normalized;
            var currentDirection = desiredDirection;

            // Get all colliders in vicinity.
            var hits = Physics2D.OverlapCircleAll(ourPos, 5);
            DrawCircle(ourPos, 5f, Color.white);
            foreach (var hit in hits)
            {
                if (hit.gameObject == this.gameObject) continue;
                if (hit.gameObject.CompareTag("Player"))
                {
                    DrawBoxCenter(hit.bounds.center, hit.bounds.size, Color.red);
                }
                else
                {
                    if (hit is TilemapCollider2D)
                    {
                        var point = hit.ClosestPoint(ourPos);
                        DrawBoxCenter(point, new Vector2(1, 1), Color.red);
                    }
                    else
                    {
                        DrawBoxCenter(hit.bounds.center, hit.bounds.size, Color.red);
                    }
                }
            }

            for (int i = 0; i < 18; i++)
            {
                currentDirection = desiredDirection.Rotate(i * 20).normalized;
                var dot = Vector2.Dot(currentDirection, desiredDirection);
                var distance = (dot + 1) / 2;
                var dist2 = distance * distance;
                var dist4 = dist2 * dist2;

                var length = (13.924 * dist4 - 18.718 * dist2 * distance + 8.2695 * dist2 - 0.4807 * distance) + 1.0f;
                var finalPos = ourPos.ToVector2() + (currentDirection * (float)length);

                if (dot >= 0)
                {
                    Debug.DrawLine(ourPos, finalPos, Color.green);
                }
                else
                {
                    Debug.DrawLine(ourPos, finalPos, Color.red);
                }

                this.directions[i] = new MovementDirection(currentDirection, (float)length, i);
            }

            var agent = this.GetComponent<NavMeshAgent>();
            var objectiveDistance = Vector3.Distance(ourPos, this.objectivePosition);
            var stopDistance = 3;
            agent.isStopped = objectiveDistance < stopDistance;

            var velocityInput = agent.velocity;
            if (agent.velocity.magnitude == 0)
            {
                velocityInput = this.lastVelocity;
            }
            else
            {
                this.lastVelocity = velocityInput;
            }

            var animator = this.GetComponent<Animator>();
            animator.SetFloat("Horizontal", velocityInput.x);
            animator.SetFloat("Vertical", velocityInput.y);

            if (this.lastPositionUpdate.HasTimeElapsed(2) || !agent.hasPath)
            {
                if (objectiveDistance > stopDistance)
                {
                    // Pick one of the best 5 directions randomly. TODO: Use weights.
                    var bestPaths = this.directions.OrderByDescending(dir => dir.Length);
                    var directionRand = Random.Range(0, 5);
                    var direction = bestPaths.Skip(directionRand).First();
                    var frontPos = ourPos + direction.Direction.ToVector3();

                    var targetPosition = frontPos + (direction.Direction.ToVector3() * 10);
                    this.currentPosition = targetPosition;
                    agent.SetDestination(targetPosition);
                    agent.isStopped = false;

                    this.lastPositionUpdate = GameTime.Now;
                }
            }

            Debug.DrawLine(ourPos, this.currentPosition, Color.white);
        }

        /// <summary>
        /// Draws a Circle using Debug.Draw functions
        /// </summary>
        /// <param name="center">Center point.</param>
        /// <param name="radius">Radius of the circle.</param>
        /// <param name="color">Color for Debug.Draw.</param>
        /// <param name="numSegments">Number of segments for the circle, used for precision of the draw.</param>
        /// <param name="duration">Duration to show the circle.</param>
        public static void DrawCircle(Vector2 center, float radius, Color color, float numSegments = 40, float duration = 0.01f)
        {
            Quaternion rotQuaternion = Quaternion.AngleAxis(360.0f / numSegments, Vector3.forward);
            Vector2 vertexStart = new Vector2(radius, 0.0f);
            for (int i = 0; i < numSegments; i++)
            {
                Vector2 rotatedPoint = rotQuaternion * vertexStart;

                // Draw the segment, shifted by the center
                Debug.DrawLine(center + vertexStart, center + rotatedPoint, color, duration);

                vertexStart = rotatedPoint;
            }
        }

        public static void DrawBoxCenter(Vector2 center, Vector2 size, Color color, float duration = 0.01f)
        {
            Vector2 worldTopLeft = new Vector2(center.x - size.x / 2, center.y - size.y / 2);
            Vector2 worldBottomRight = new Vector2(center.x + size.x / 2, center.y + size.y / 2);
            DrawBox(worldTopLeft, worldBottomRight, color, duration);
        }

        /// <summary>
        /// Draws a box using Debug.Draw functions
        /// </summary>
        /// <param name="worldTopLeft">World top left corner.</param>
        /// <param name="worldBottomRight">World bottom right corner.</param>
        /// <param name="color">Color for Debug.Draw.</param>
        /// <param name="duration">Duration to show the box.</param>
        public static void DrawBox(Vector2 worldTopLeft, Vector2 worldBottomRight, Color color, float duration = 0.01f)
        {
            Vector2 worldTopRight = new Vector2(worldBottomRight.x, worldTopLeft.y);
            Vector2 worldBottomLeft = new Vector2(worldTopLeft.x, worldBottomRight.y);

            Debug.DrawLine(worldTopLeft, worldBottomLeft, color, duration);
            Debug.DrawLine(worldBottomLeft, worldBottomRight, color, duration);
            Debug.DrawLine(worldBottomRight, worldTopRight, color, duration);
            Debug.DrawLine(worldTopRight, worldTopLeft, color, duration);
        }
    }

    class MovementDirection
    {
        public MovementDirection(Vector2 direction, float length, int dirIndex)
        {
            Direction = direction;
            Length = length;
            DirIndex = dirIndex;
        }

        public Vector2 Direction { get; }
        public float Length { get; }
        public int DirIndex { get; }
    }
}
