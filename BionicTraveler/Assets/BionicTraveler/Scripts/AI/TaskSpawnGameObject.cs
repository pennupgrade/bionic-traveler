namespace BionicTraveler.Scripts.AI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BionicTraveler.Scripts.World;
    using UnityEngine;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TaskSpawnGameObject : EntityTask
    {
        private float spawnRadius;
        private List<GameObject> spawnableObjects;
        private Func<List<GameObject>, GameObject> select;

        public TaskSpawnGameObject(DynamicEntity owner, List<GameObject> spawnableObjects, Func<List<GameObject>, GameObject> select, float radius = 0) : base(owner)
        {
            spawnRadius = radius;
            this.spawnableObjects = spawnableObjects;
            this.select = select;
        }

        public override EntityTaskType Type => EntityTaskType.Spawn;

        public override void OnProcess()
        {
            var newPosition = Owner.transform.position;
            newPosition += (Vector3)(UnityEngine.Random.insideUnitCircle * this.spawnRadius);
            this.SpawnAt(newPosition);
            this.End("Spawned Succesfully!", true);
        }

        private void SpawnAt(Vector3 pos)
        {
            GameObject g = UnityEngine.Object.Instantiate(select(spawnableObjects));
            g.transform.position = pos;
            g.SetActive(true);
        }
    }
}
