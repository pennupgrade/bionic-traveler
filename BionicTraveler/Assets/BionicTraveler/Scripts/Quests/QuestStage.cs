namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class UnityInheritanceAttribute : PropertyAttribute
    {
    }

    public interface UnityInheritance
    {
        IEnumerable<Type> DiscoverTypes();

        public string Type { get; set; }

        public object CreateType(string typeName);

        public string InstanceName { get; }
    }

    [System.Serializable]
    public class UnityInheritance<T2> : UnityInheritance where T2 : class, new()
    {
        private string typeName;

        [SerializeReference]
        public T2 Instance;

        public string InstanceName => nameof(this.Instance);

        public string Type
        {
            get => this.typeName;
            set
            {
                if (this.typeName != value)
                {
                    this.Instance = null;
                }

                this.typeName = value;
            }
        }

        public object CreateType(string typeName) => this.CreateGenericType(typeName);

        public T2 CreateGenericType(string typeName)
        {
            if (this.Instance != null)
            {
                return this.Instance;
            }

            var types = this.DiscoverTypes();
            var type = types.FirstOrDefault(t => t.FullName == typeName);
            this.Instance = Activator.CreateInstance(type) as T2;
            return this.Instance;
        }

        public IEnumerable<Type> DiscoverTypes()
        {
            return typeof(T2).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T2)) && !t.IsAbstract);
        }
    }

    /// <summary>
    /// Please document me.
    /// Explore the Temple in the Wilderness.
    /// </summary>
    //[CreateAssetMenu(fileName = "MyNewQuestStage", menuName = "Quests/QuestStage")]
    [Serializable]
    public class QuestStage
    {
        [SerializeField]
        private string description;

        [SerializeField]
        [UnityInheritanceAttribute]
        private UnityInheritance<QuestObjective> objective;

        public bool IsComplete()
        {
            return this.objective.Instance.IsComplete();
        }
    }
}
