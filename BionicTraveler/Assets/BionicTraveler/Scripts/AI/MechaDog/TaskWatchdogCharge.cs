namespace BionicTraveler.Scripts.AI.MechaDog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TaskWatchdogCharge : TaskAnimated
    {
        private float initialSpeed;
        private Vector2 direction;
        Entity target;
        GameObject coll;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDash"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TaskWatchdogCharge(DynamicEntity owner, Entity target, GameObject coll)
            : base(owner)
        {
            this.initialSpeed = 15f;
            this.target = target;
            this.coll = GameObject.Instantiate(coll);
        }

        /// <inheritdoc/>
        public override EntityTaskType Type => EntityTaskType.WatchdogCharge;

        /// <inheritdoc/>
        public override void OnInitialize()
        {
            if (this.HasAnimation("Dashing"))
            {
                this.PlayAnimation("Dashing");
            }
            else
            {
                this.End("Entity has no dashing animation", false);
            }
        }

        /// <inheritdoc/>
        public override void OnProcess()
        {

            this.direction = (this.target.transform.position - this.Owner.transform.position).normalized;

            if (this.HasCurrentAnimationFinished())
            {
                this.End("Charge complete!", true);
            }
            else
            {
                if (this.IsCurrentAnimationPlaying())
                {
                    var rb = this.Owner.GetComponent<Rigidbody2D>();
                    var state = this.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    var speed = Mathf.Lerp(this.initialSpeed, 0, state);
                    rb.MovePosition(rb.position + (this.direction * speed * Time.fixedDeltaTime));
                }
            }
        }

        /// <inheritdoc/>
        public override void OnEnd()
        {
            this.PlayAnimation("Idle");
            GameObject.Destroy(coll);
            base.OnEnd();
        }

    }
}
