namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Entity Task Manager class keeps track of one entity's list
    /// of tasks
    /// </summary>
    public class EntityTaskManager
    {
        
        private List<EntityTask> tasks;
        private DynamicEntity owner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public EntityTaskManager(DynamicEntity owner)
        {
            this.owner = owner;
            this.tasks = new List<EntityTask>();
        }

        /// <summary>
        /// Blah
        /// </summary>
        /// <param name="task"></param>
        public void Assign(EntityTask task)
        {
            this.tasks.Add(task);
        }

        /// <summary>
        /// Returns active tasks
        /// </summary>
        /// <returns>An array of EntityTask </returns>
        public EntityTask[] GetActiveTasks()
        {
            return this.tasks.Where(task => task.IsActive).ToArray();
        }

        /// <summary>
        /// Return a list of inactive tasks
        /// </summary>
        /// <returns>An Entity Task Array</returns>
        public EntityTask[] GetInactiveTasks()
        {
            return this.tasks.Where(task => !task.IsActive).ToArray();
        }

        /// <summary>
        /// Removes all tasks
        /// </summary>
        public void ClearTasks()
        {
            foreach (EntityTask task in this.tasks) {
                task.Cancel("ClearTasks()");
            }

            this.tasks.Clear();
        }

        /// <summary>
        /// Removes the first active task of that type.
        /// </summary>
        /// <param name="type">Type of the Task</param>
        public void ClearTask(EntityTaskType type)
        {
            // If task is being executed currently, wait for it to end.
            foreach (EntityTask task in this.tasks)
            {
                if (task.GetType().Equals(type))
                {
                    // Is it possible to remove a task from list w/o ending it?
                    this.tasks.Remove(task);
                    return;
                }
            }

            // If task does not exist, throw exception?
            throw new Exception("Task type does not exist");
        }

        /// <summary>
        /// Returns first task of the specified type.
        /// Return null if none of the task type exists.
        /// </summary>
        /// <param name="type">Type of the Task</param>
        /// <returns> An instance of EntityTask </returns>
        public EntityTask FindTaskByType(EntityTaskType type)
        {
            return this.tasks.Find(task => task.Type == type);
        }

        /// <summary>
        /// Returns all Tasks of the specified type.
        /// </summary>
        /// <param name="type">Type of the Task</param>
        /// <returns> An array of EntityTask </returns>
        public EntityTask[] FindTasksByType(EntityTaskType type)
        {
            return this.tasks.Where(task => task.Type == type).ToArray();
        }

        /// <summary>
        /// Returns true if there is an active task of the type.
        /// </summary>
        /// <param name="type">Type of the Task</param>
        /// <returns> boolean </returns>
        public bool IsTaskActive(EntityTaskType type)
        {
            EntityTask firstTaskOfType = this.FindTaskByType(type);
            if (firstTaskOfType != null)
            {
                return firstTaskOfType.IsActive;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Main loop process for EntityTaskManager
        /// Promotes tasks, clears inactive tasks, etc.
        /// Called each time after EntityTask death/cancellation
        /// </summary>
        public void Process()
        {
            // Removes inactive tasks iff they have ended
            EntityTask[] inactiveTasks = this.GetInactiveTasks();
            foreach (EntityTask task in inactiveTasks)
            {
                if (task.HasEnded)
                {
                    this.tasks.Remove(task);
                }
            }

            // Loop over a current copy of all tasks in an array
            // execute every task

            foreach (EntityTask task in this.tasks.ToArray())
            {
                task.Process();
            }

            // at this point, tasks are either still active or done
            // will be cleaned on the next tick
            // may have new tasks
        }

        public void ShutDown()
        {
            Debug.Log("TaskManager shut down");
            this.ClearTasks();
        }
    }
}
