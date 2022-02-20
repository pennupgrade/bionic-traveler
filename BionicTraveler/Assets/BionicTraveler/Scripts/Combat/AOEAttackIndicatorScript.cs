namespace BionicTraveler.Scripts.Combat
{
    using BionicTraveler.Assets.Framework;
    using BionicTraveler.Scripts.World;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class AOEAttackIndicatorScript : MonoBehaviour
    {
        private GameTime spawnedTime;
        private Transform tgtTransform;

        [SerializeField]
        private GameObject aoeAttack;

        [SerializeField]
        private AttackData aoeAttackData;

        [SerializeField]
        private float trackTime;

        [SerializeField]
        private float triggerTime;

        private DynamicEntity owner;

        private Vector3 initialScale;
        private GameObject currentAttack;

        public void SetOwner(DynamicEntity o)
        {
            this.owner = o;
            this.owner.Died += Owner_Died;
        }

        private void Owner_Died(Entity sender, Entity killer)
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (this.currentAttack != null && !this.currentAttack.IsDestroyed())
            {
                Destroy(this.currentAttack);
            }
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
                this.gameObject.transform.position = this.tgtTransform.position;

                this.gameObject.transform.localScale = Vector3.Lerp(
                    this.gameObject.transform.localScale,
                    this.initialScale,
                    0.04f);
            }

            if (this.spawnedTime.HasTimeElapsed(this.triggerTime) && this.currentAttack == null)
            {
                this.Triggered();
            }
        }

        public void SetTargetPosition(Transform tgt)
        {
            this.tgtTransform = tgt;
        }

        public void Triggered()
        {
            Destroy(this.gameObject);
            this.currentAttack = GameObject.Instantiate(this.aoeAttack, this.gameObject.transform.position, Quaternion.identity);
            this.currentAttack.GetComponent<AoeAttack>().SetData(this.aoeAttackData);
            this.currentAttack.GetComponent<AoeAttack>().StartAttack(this.owner);
        }
    }
}
