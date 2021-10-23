namespace BionicTraveler.Scripts.Combat
{
    using UnityEngine;

    /// <summary>
    /// Test class to debug weapons behavior.
    /// </summary>
    public class WeaponsTest : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The weapon data to use.")]
        private WeaponData weaponData;

        [SerializeField]
        [Tooltip("The second weapon data to use.")]
        private WeaponData weaponDataSecond;

        private CombatBehavior combat;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            this.combat = this.gameObject.AddComponent<CombatBehavior>();
            this.combat.SetWeapon(this.weaponData);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                this.combat.SetWeapon(this.weaponDataSecond);
            }
        }
    }
}
