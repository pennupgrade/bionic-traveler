namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    class TaskAttack : TaskAnimated
    {
        private readonly WeaponBehaviour weaponBehavior;
        private readonly bool isPrimaryWeaponMode;
        private bool hasFired;
        private bool skipAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAttack"/> class.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="weaponBehavior"></param>
        /// <param name="primary"></param>
        public TaskAttack(DynamicEntity owner, WeaponBehaviour weaponBehavior, bool primary)
            : base(owner)
        {
            this.weaponBehavior = weaponBehavior;
            this.isPrimaryWeaponMode = primary;
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Attack;

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (!this.hasFired)
            {
                this.weaponBehavior.SetWeaponMode(this.isPrimaryWeaponMode);
                var attackData = this.weaponBehavior.GetNextAttackData();
                if (this.HasAnimation(attackData.AnimationState))
                {
                    this.PlayAnimation(attackData.AnimationState);
                }
                else
                {
                    Debug.LogWarning($"{this.weaponBehavior.WeaponData.Id} has no animation!");
                    this.skipAnimation = true;
                }

                this.weaponBehavior.Fire(this.Owner);
                this.hasFired = true;
            }
            else
            {
                if (this.skipAnimation || this.HasCurrentAnimationFinished())
                {
                    this.End("Attack animation has finished", true);
                }
            }
        }
    }
}