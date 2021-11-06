namespace BionicTraveler.Scripts.Combat
{
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Describes an instant melee attack.
    /// </summary>
    public class MeleeAttack : Attack
    {
        private bool hasAttacked;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            // TODO: Improve target detection.
            var enemies = GameObject.FindGameObjectsWithTag("Enemy").Where(
                obj => obj.GetComponent<DynamicEntity>() != null);

            var ourPos = this.gameObject.transform.position;
            var enemiesInRange = enemies.Where(enemy => Vector3.Distance(enemy.transform.position, ourPos)
                                                < this.AttackData.Range);

            return enemiesInRange.Select(enemy => (Entity)enemy.GetComponent<DynamicEntity>()).ToArray();
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            foreach (var target in targets)
            {
                target.OnHit(this);
            }

            this.hasAttacked = true;
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            return this.hasAttacked;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }
    }
}
