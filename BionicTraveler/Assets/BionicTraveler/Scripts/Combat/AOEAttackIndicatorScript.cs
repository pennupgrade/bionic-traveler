namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class AoeAttackIndicatorScript : MonoBehaviour
    {
        private GameTime spawnedTime;
        private Transform tgtTransform;

        [SerializeField]
        private AttackData aoeAttackData;

        [SerializeField]
        private float trackTime;

        [SerializeField]
        private float triggerTime;

        private DynamicEntity owner;

        private Vector3 initialScale;
        private Attack currentAttack;

        public void Initialize(DynamicEntity owner, Transform target)
        {
            this.owner = owner;
            this.owner.Died += this.Owner_Died;
            this.tgtTransform = target;
        }

        private void Owner_Died(Entity sender, Entity killer)
        {
            Destroy(this);
        }

        public void Start()
        {
            this.spawnedTime = GameTime.Now;
            this.initialScale = this.gameObject.transform.localScale;
            this.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (!this.spawnedTime.HasTimeElapsed(this.trackTime))
            {
                this.GetComponent<SpriteRenderer>().enabled = true;
                this.gameObject.transform.position = this.tgtTransform.position;

                this.gameObject.transform.localScale = Vector3.Lerp(
                    Vector3.zero,
                    this.initialScale,
                    this.spawnedTime.GetElapsedNormalized(this.trackTime));
            }

            if (this.spawnedTime.HasTimeElapsed(this.triggerTime) && this.currentAttack == null)
            {
                this.Triggered();
            }
        }

        public void Triggered()
        {
            var attack = AttackFactory.CreateAttack(this.aoeAttackData) as AoeAttack;
            this.currentAttack = attack;
            this.currentAttack.StartAttack(this.owner);
            attack.SetOrigin(this.gameObject.transform.position);

            Destroy(this.gameObject);
        }
    }
}
