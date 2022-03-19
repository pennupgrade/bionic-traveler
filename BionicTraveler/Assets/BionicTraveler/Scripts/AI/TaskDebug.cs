namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.Combat;
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TaskDebug : MonoBehaviour
    {
        public DynamicEntity entity1;
        public Entity entity2;

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        public void Start()
        {
            //var task = new FollowEntityTask(this.entity1, this.entity2);
            //this.entity1.TaskManager.Assign(task);

            //Vector3 entity1Pos = this.entity1.transform.position;

            //// when Ended event happens, proceed to bracketed code
            //task.Ended += delegate {
            //    var task2 = new GoToPointTask(this.entity1, entity1Pos);
            //    this.entity1.TaskManager.Assign(task2);
            //};

            //var task = new TaskPatrol(this.entity1, PatrolType.Square);
            //this.entity1.TaskManager.Assign(task);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        public void Update()
        {
            if (Input.GetButtonDown("[Debug] Assign Attack"))
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                var playerEntity = player.GetComponent<PlayerEntity>();
                var taskAttack = new TaskAttack(playerEntity, true);
                taskAttack.Assign();

                //this.entity1.TaskManager.ClearTasks();
            }

            if (Input.GetButtonDown("[Debug] Assign Move"))
            {
                this.entity1.TaskManager.ClearTasks();

                var sequence = new TaskSequence();
                var startPos = this.entity1.transform.position;
                sequence.AddTasks(new TaskMoveToEntity(this.entity1, this.entity2), new TaskGoToPoint(this.entity1, startPos));
                sequence.Execute(this.entity1);
            }
        }
    }
}
