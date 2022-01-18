namespace BionicTraveler.Scripts.Quests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Describes the quest state.
    /// </summary>
    public enum QuestState
    {
        /// <summary>
        /// No state. This is a development only state.
        /// </summary>
        None,

        /// <summary>
        /// The active quest that is being tracked.
        /// </summary>
        Active,

        /// <summary>
        /// An inactive quest that still processes events.
        /// </summary>
        Inactive,

        /// <summary>
        /// A finished quest that no longer processes events.
        /// </summary>
        Finished,
    }

    /// <summary>
    /// Main class to describe a quest. Consists of multiple stages.
    /// For instance: Explore all Temples in the Universe.
    /// </summary>
    [CreateAssetMenu(fileName = "MyNewQuest", menuName = "Quests/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField]
        private string title;

        [SerializeField]
        private string description;

        [SerializeField]
        private List<QuestStage> stages;

        // Since this is a ScriptableObject, all state is serialized by default. We disable this for the
        // following fields so they get reset on each start.
        [System.NonSerialized]
        private QuestStage activeStage;
        [System.NonSerialized]
        private int activeStageIndex;
        [System.NonSerialized]
        private QuestState state;

        /// <summary>
        /// Delegate for changes in a <see cref="Quest"/>'s state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newState">The new state.</param>
        public delegate void QuestStateChangedEventHandler(Quest sender, QuestState newState);

        /// <summary>
        /// Invoked when the quest state changes.
        /// </summary>
        public event QuestStateChangedEventHandler StateChanged;

        /// <summary>
        /// Gets a value indicating whether this quest is complete.
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Initializes the quest and all its related stages.
        /// </summary>
        /// <param name="questManager">The quest manager.</param>
        public void Initialize(QuestManager questManager)
        {
            if (this.stages.Count == 0)
            {
                throw new InvalidOperationException("Quest has no stages.");
            }

            this.activeStageIndex = -1;
            this.ProceedToNextStage();
        }

        /// <summary>
        /// Sets the quests state to active or inactive.
        /// </summary>
        /// <param name="active">Whether or not the quest is active.</param>
        public void SetAsActiveQuest(bool active)
        {
            if (this.state == QuestState.Finished)
            {
                throw new InvalidOperationException("Cannot change state of finished quest.");
            }

            this.SetState(active ? QuestState.Active : QuestState.Inactive);
        }

        /// <summary>
        /// Processes a new quest event.
        /// </summary>
        /// <param name="questEvent">The event.</param>
        public void ProcessEvent(QuestEvent questEvent)
        {
            this.activeStage.ProcessEvent(questEvent);
        }

        private bool ProceedToNextStage()
        {
            if (this.activeStage != null)
            {
                this.activeStage.Objective.CompleteStateChanged -= this.Objective_CompleteStateChanged;
            }

            this.activeStageIndex++;
            if (this.activeStageIndex < this.stages.Count)
            {
                this.activeStage = this.stages[this.activeStageIndex];
                this.activeStage.Objective.CompleteStateChanged += this.Objective_CompleteStateChanged;
                this.activeStage.Initialize();
                return true;
            }

            return false;
        }

        private void Objective_CompleteStateChanged(QuestObjective sender, bool newState)
        {
            Debug.Log("Quest stage new state: " + newState);

            // No more stages, finish quest.
            if (!this.ProceedToNextStage())
            {
                this.IsComplete = true;
                this.SetState(QuestState.Finished);
            }
        }

        private void SetState(QuestState state)
        {
            this.state = state;
            this.StateChanged?.Invoke(this, state);
        }
    }
}
