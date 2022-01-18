namespace BionicTraveler.Scripts.Quests
{
    using System;
    using Framework;
    using UnityEngine;

    /// <summary>
    /// Describes a stage in a quest which consists of a description and one objective.
    /// For instance: Explore the Temple in the Wilderness.
    /// </summary>
    [Serializable]
    public class QuestStage : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string description;

        [SerializeField]
        [UnityInheritanceAttribute]
        private UnityInheritance<QuestObjective> objective;

        /// <summary>
        /// Gets a value indicating whether this quest stage is complete.
        /// </summary>
        public bool IsComplete => this.objective.Instance.IsComplete;

        /// <summary>
        /// Gets the objective of the quest stage.
        /// </summary>
        public QuestObjective Objective => this.objective.Instance;

        /// <summary>
        /// Initializes the quest stage.
        /// </summary>
        public void Initialize()
        {
            this.objective.Instance.Activate();
        }

        /// <summary>
        /// Processes a new quest event.
        /// </summary>
        /// <param name="questEvent">The event.</param>
        public virtual void ProcessEvent(QuestEvent questEvent)
        {
            // Use double dispatching to send this event to the handler with the best matching overload.
            // If no overload can be found, this will hit the base handler.
            // This is a relatively slow call due to late binding.
            // ((dynamic)this.objective.Instance).ProcessEvent((dynamic)questEvent);

            // Use fast reflection to avoid hit on first late-bound call (above call is very slow on first invocation).
            var method = this.objective.Instance.GetType().GetMethod(nameof(this.ProcessEvent), new Type[] { questEvent.GetType() });
            new FastMethodInfo(method).Invoke(this.objective.Instance, questEvent);
        }

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            // VERY VERY IMPORTANT! If we do not call this, adding new items via the Unity inspector
            // will create a copy of our last item (so far, so good). However, it will contain the same
            // reference for our UnityInheritance internal instance so that any changes to our objective
            // carry over to copied instance. We force a freshly cloned instance here to ensure we get
            // a new reference. Not great, but works :-)
            this.objective.RefreshInstance();
        }
    }
}
