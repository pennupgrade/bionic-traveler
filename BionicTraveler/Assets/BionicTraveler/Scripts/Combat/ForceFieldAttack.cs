namespace BionicTraveler.Scripts.Combat
{
    using System.Linq;
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Describes a defensive force field that deflects any of kinds of attack (melee and ranged)
    /// </summary>
    public class ForceFieldAttack : Attack
    {
        private GameTime attackStart;

        /// <inheritdoc/>
        public override Entity[] GetTargets()
        {
            EnemyScanner scanner = this.Owner.EnemyScanner;
            GameObject[] enemies = scanner.GetEnemies();
            var ourPos = this.gameObject.transform.position;
            var enemiesInRange = enemies.Where(enemy => Vector3.Distance(enemy.transform.position, ourPos)
                                                        < this.AttackData.Range);

            return enemiesInRange.Select(enemy => (Entity)enemy.GetComponent<DynamicEntity>()).ToArray();
        }

        /// <inheritdoc/>
        public override void AttackTargets(Entity[] targets)
        {
            var ourPos = this.gameObject.transform.position;

            // Calculates the force for each target in targets based on their distance away from the circumference of
            // attack range circle. The closer a target is, the stronger the force is applied. (Use a minimum static
            // force to ensure objects right on the circumference are visibly pushed back).
            foreach (var target in targets)
            {
                var targetPos = target.transform.position;
                var targetVector = (targetPos - ourPos).normalized;
                var outerPosition = ourPos + (targetVector * this.AttackData.Range);
                var forceVector = (outerPosition - targetPos) + (targetVector * 1.8f);

                // TODO: Read force from attack template.
                Vector2 force = forceVector * 3;
                if (target.IsDynamic)
                {
                    ((DynamicEntity)target).ApplyForce(force);
                }
            }
        }

        /// <inheritdoc/>
        public override bool HasFinished()
        {
            // TODO: Implement battery drain logic.
            return this.attackStart.HasTimeElapsed(5.0f);
        }

        /// <inheritdoc/>
        public override void OnAttackStarted()
        {
            base.OnAttackStarted();
            this.attackStart = GameTime.Now;
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
        }
    }
}
