namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Performs a single attack from an entity.
    /// </summary>
    public class TaskAttack : TaskAnimated
    {
        private readonly WeaponBehaviour weaponBehavior;
        private readonly bool isPrimaryWeaponMode;
        private bool hasFired;
        private bool skipAnimation;
        private Animator weaponAnimator;
        private AttackData attackData;
        private string currentAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAttack"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="primary">Whether the primary or secondary attack should be used.</param>
        public TaskAttack(DynamicEntity owner, bool primary)
            : base(owner)
        {
            this.isPrimaryWeaponMode = primary;
            this.weaponBehavior = this.Owner.WeaponsInventory.equippedWeaponBehavior;
            this.Owner.WeaponsInventory.DisplayCurrentWeapon();
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.Attack;

        /// <inheritdoc/>
        public override void OnProcess()
        {
            if (!this.hasFired)
            {
                this.weaponBehavior.SetWeaponMode(this.isPrimaryWeaponMode);
                this.weaponAnimator = this.weaponBehavior.WorldObject.GetComponent<Animator>();

                this.attackData = this.weaponBehavior.GetNextAttackData();
                var animationToUse = this.attackData.AnimationState;
                if (!string.IsNullOrWhiteSpace(animationToUse))
                {
                    bool hasTwoAnimations = !string.IsNullOrEmpty(this.attackData.AnimationStateLeft);
                    if (hasTwoAnimations)
                    {
                        if (this.Owner.IsFacingLeft())
                        {
                            animationToUse = this.attackData.AnimationStateLeft;
                        }
                    }

                    if (this.weaponAnimator.HasAnimation(animationToUse))
                    {
                        this.currentAnimation = animationToUse;
                        this.weaponAnimator.Play(this.currentAnimation);
                    }
                    else
                    {
                        Debug.LogWarning($"{this.weaponBehavior.WeaponData.Id} has no animation {animationToUse}!");
                        this.skipAnimation = true;
                    }
                }
                else
                {
                    this.skipAnimation = true;
                }

                this.weaponBehavior.Fire(this.Owner);
                this.hasFired = true;
            }
            else
            {
                if (this.skipAnimation || this.weaponAnimator.HasAnimationFinished(this.currentAnimation))
                {
                    this.End("Attack animation has finished", true);
                }
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            base.OnEnd();
            this.weaponBehavior?.StoppedAttack();
        }
    }
}