namespace BionicTraveler.Scripts
{
    using BionicTraveler.Scripts.AI;
    using BionicTraveler.Scripts.Combat;
    using UnityEngine;

    /// <summary>
    /// Describes default game config settings that apply to all entities that have no overriding settings.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewGameConig", menuName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        private WeaponData defaultUnarmedWeapon;

        [SerializeField]
        private EntityRelationships defaultEntityRelations;

        /// <summary>
        /// Gets the default unarmed weapon for entities.
        /// </summary>
        public WeaponData DefaultUnarmedWeapon => this.defaultUnarmedWeapon;

        /// <summary>
        /// Gets the default entity relationships.
        /// </summary>
        public EntityRelationships DefaultEntityRelations => this.defaultEntityRelations;
    }
}
