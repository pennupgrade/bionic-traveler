namespace BionicTraveler.Scripts.Combat
{
    using UnityEngine;

    /// <summary>
    /// Describes an attack that a weapon can possess.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewAttack", menuName = "Attacks/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The name of the attack.")]
        private string displayName;

        [SerializeField]
        [Tooltip("The type of the attack.")]
        private AttackType type;

        [SerializeField]
        [Tooltip("How much damage weapon has on other characters.")]
        private float damage;

        [SerializeField]
        [Tooltip("Distance a weapon has effect over upon activation.")]
        private float range;

        [SerializeField]
        [Tooltip("Duration between each activation of weapon.")]
        private float cooldown;

        [SerializeField]
        [Tooltip("How long character is frozen when activating weapon.")]
        private float freezeDuration;

        [SerializeField]
        [Tooltip("Energy that one heavy attack/defense mechanic consumes.")]
        private float cost;

        [SerializeField]
        [Tooltip("Time it takes to reload a weapon.")]
        private float reloadTime;

        [SerializeField]
        [Tooltip("Maximum number of ammunition for weapon.")]
        private float maxAmmo;

        [SerializeField]
        [Tooltip("Maximum number of ammunition per magazine.")]
        private int clipSize;

        [SerializeField]
        [Tooltip("Angle of effect of weapon.")]
        private float fovAngle;

        [SerializeField]
        [Tooltip("Area that the defense mechanics acts upon.")]
        private float areaOfEffect;

        [SerializeField]
        [Tooltip("Art sprite for the attack.")]
        private Sprite sprite;

        [SerializeField]
        [Tooltip("Animation of the attack.")]
        private Animation animation;

        // Projectile data, perhaps move to separate ProjectileData at some point. TODO: Evaluate.
        [SerializeField]
        [Tooltip("Speed of the projectile that the weapon ejects.")]
        private float projectileSpeed;

        [SerializeField]
        [Tooltip("Should the projectile entity get destroyed on hit or not.")]
        private bool projectileDestroyOnImpact;

        /// <summary>
        /// Gets the display name of the attack.
        /// </summary>
        public string DisplayName => this.displayName;

        /// <summary>
        /// Gets the type of the attack.
        /// </summary>
        public AttackType Type => this.type;

        /// <summary>
        /// Gets the amount of damage this attack deals.
        /// </summary>
        public float Damage => this.damage;

        /// <summary>
        /// Gets the range of this attack. Targets outside the range cannot be hit.
        /// </summary>
        public float Range => this.range;

        /// <summary>
        /// Gets the cooldown in seconds between using this attack again.
        /// </summary>
        public float Cooldown => this.cooldown;

        /// <summary>
        /// Gets the amount of time that character is frozen when activating this attack.
        /// </summary>
        public float FreezeDuration => this.freezeDuration;

        /// <summary>
        /// Gets the energy cost to perform this attack.
        /// </summary>
        public float Cost => this.cost;

        /// <summary>
        /// Gets the reload time in seconds.
        /// </summary>
        public float ReloadTime => this.reloadTime;

        /// <summary>
        /// Gets the maximum amount of ammo that can be carried.
        /// </summary>
        public float MaxAmmo => this.maxAmmo;

        /// <summary>
        /// Gets the amount of ammunition per magazine.
        /// </summary>
        public int ClipSize => this.clipSize;

        /// <summary>
        /// Gets the field of view angle this attack uses to target entities.
        /// </summary>
        public float FovAngle => this.fovAngle;

        /// <summary>
        /// Gets the area of effect distance.
        /// </summary>
        public float AreaOfEffect => this.areaOfEffect;

        /// <summary>
        /// Gets the attack's sprite.
        /// </summary>
        public Sprite Sprite => this.sprite;

        /// <summary>
        /// Gets the animation this attack uses.
        /// </summary>
        public Animation Animation => this.animation;

        /// <summary>
        /// Gets the speed of the projectile.
        /// </summary>
        public float ProjectileSpeed => this.projectileSpeed;

        /// <summary>
        /// Gets a value indicating whether the projectile gets destroyed on impact.
        /// </summary>
        public bool ProjectileDestroyOnImpact => this.projectileDestroyOnImpact;

        /// <summary>
        /// Calculates the attack's base damage. Currently just returns <see cref="this.Damage"/>.
        /// </summary>
        /// <returns>The attack's base damage.</returns>
        public float GetBaseDamage()
        {
            return this.Damage;
        }
    }
}
