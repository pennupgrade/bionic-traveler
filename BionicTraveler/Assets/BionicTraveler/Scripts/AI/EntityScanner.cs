namespace BionicTraveler.Scripts.AI
{
    using System.Collections.Generic;
    using System.Linq;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// A poor man's scene graph to keep track of entities around an entity to support quick
    /// query operations. Uses collision to detect entities.
    /// </summary>
    public class EntityScanner : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The detection range of the entity.")]
        private int detectionRange;

        private CircleCollider2D rangeCollider;
        private List<Entity> entitiesInRange;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.entitiesInRange = new List<Entity>();

            // Add our collider.
            this.rangeCollider = this.gameObject.AddComponent<CircleCollider2D>();
            this.rangeCollider.isTrigger = true;
            this.rangeCollider.offset = Vector2.zero;
            this.rangeCollider.radius = this.detectionRange;
        }

        /// <summary>
        /// Gets all entities within detection range of this entity scanner.
        /// </summary>
        /// <returns>An array of entities.</returns>
        public Entity[] GetAllInRange() => this.entitiesInRange.ToArray();

        /// <summary>
        /// Gets all dynamicentities within detection range of this entity scanner.
        /// </summary>
        /// <returns>An array of dynamic entities.</returns>
        public DynamicEntity[] GetAllDynamicInRange() => this.entitiesInRange.Where(entity
            => entity.IsDynamic).Cast<DynamicEntity>().ToArray();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                this.entitiesInRange.Add(entity);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entity = collision.GetComponent<Entity>();
            if (entity != null)
            {
                this.entitiesInRange.Remove(entity);
            }
        }

        /// <summary>
        /// Draws a sphere showing the seeing area.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, this.detectionRange);
        }
    }
}
